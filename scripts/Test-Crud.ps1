param([string]$BaseUrl = 'http://localhost:5080')
$ErrorActionPreference = 'Stop'
$session = [Microsoft.PowerShell.Commands.WebRequestSession]::new()
$title = 'CRUD check ' + [guid]::NewGuid().ToString('N').Substring(0, 10)
$bookId = $null

function Get-Page([string]$Path) {
    Invoke-WebRequest ($BaseUrl + $Path) -WebSession $session -SkipHttpErrorCheck
}

function Get-Token($Response) {
    $token = [regex]::Match($Response.Content, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"').Groups[1].Value
    if (-not $token) { throw 'Form is missing an anti-forgery token.' }
    return $token
}

function Post-Form([string]$Path, [hashtable]$Fields) {
    Invoke-WebRequest ($BaseUrl + $Path) -Method Post -WebSession $session -Body $Fields -SkipHttpErrorCheck
}

function Assert([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw $Message }
    Write-Output ('PASS: ' + $Message)
}

try {
    $list = Get-Page '/Books'
    Assert ($list.StatusCode -eq 200) 'Book register loads with a database connection'
    $create = Get-Page '/Books/Create'
    $fields = @{
        '__RequestVerificationToken' = Get-Token $create
        'Book.title' = $title
        'Book.author' = "O'Brien"
        'Book.publisher' = 'Assignment check'
        'Book.status' = 'Available'
        'Book.category' = 'Testing'
        'Book.description' = 'Temporary record, removed after this check.'
    }
    $created = Post-Form '/Books/Create' $fields
    Assert ($created.StatusCode -eq 200 -and $created.Content.Contains($title)) 'Create persists a book'
    $search = Get-Page ('/Books?Search=' + [uri]::EscapeDataString($title))
    $idMatch = [regex]::Match($search.Content, '/Books/Edit/(\d+)')
    Assert ($idMatch.Success -and $search.Content.Contains($title)) 'Search finds the saved title'
    $bookId = [int]$idMatch.Groups[1].Value
    $authorSearch = Get-Page '/Books?Search=O%27Brien'
    Assert ($authorSearch.Content.Contains($title)) 'Author search handles apostrophes'
    $literalSearch = Get-Page '/Books?Search=%27%20OR%201%3D1--'
    Assert (-not $literalSearch.Content.Contains($title)) 'Search treats SQL-looking input as text'

    $edit = Get-Page ('/Books/Edit/' + $bookId)
    $fields['__RequestVerificationToken'] = Get-Token $edit
    $fields['Book.Id'] = $bookId
    $fields['Book.status'] = 'Issued'
    $edited = Post-Form ('/Books/Edit/' + $bookId) $fields
    Assert ($edited.StatusCode -eq 200) 'Edit saves successfully'
    $saved = Get-Page ('/Books/Edit/' + $bookId)
    Assert ($saved.Content -match '<option value="Issued" selected="selected">') 'Edited status persists'
    $fields['Book.Id'] = $bookId + 1
    $mismatch = Post-Form ('/Books/Edit/' + $bookId) $fields
    Assert ($mismatch.StatusCode -eq 400) 'Mismatched edit IDs are rejected'
    $fields.Remove('Book.Id')

    foreach ($invalid in @(
        @{ Field='Book.title'; Value='   '; Message='Enter a book title.' },
        @{ Field='Book.author'; Value=''; Message='Enter the author' },
        @{ Field='Book.title'; Value=('x' * 51); Message='maximum length of 50' },
        @{ Field='Book.status'; Value='Unknown'; Message='Choose Available or Issued.' }
    )) {
        $invalidFields = $fields.Clone()
        $invalidFields[$invalid.Field] = $invalid.Value
        $invalidResponse = Post-Form '/Books/Create' $invalidFields
        Assert ($invalidResponse.StatusCode -eq 200 -and $invalidResponse.Content.Contains($invalid.Message)) ('Validation: ' + $invalid.Message)
    }
    $noToken = Post-Form '/Books/Create' @{}
    Assert ($noToken.StatusCode -eq 400) 'Requests without anti-forgery tokens are rejected'
    $confirm = Get-Page ('/Books/Delete/' + $bookId)
    Assert ($confirm.Content.Contains($title)) 'Delete asks for confirmation'
    $stillThere = Get-Page ('/Books/Edit/' + $bookId)
    Assert ($stillThere.StatusCode -eq 200) 'Viewing delete confirmation does not delete the book'
    $deleted = Post-Form ('/Books/Delete/' + $bookId) @{ '__RequestVerificationToken' = Get-Token $confirm }
    Assert ($deleted.StatusCode -eq 200) 'Confirmed deletion succeeds'
    $missing = Get-Page ('/Books/Edit/' + $bookId)
    Assert ($missing.StatusCode -eq 404) 'Deleted record returns 404'
    $bookId = $null
    Write-Output 'All CRUD checks passed.'
}
finally {
    if ($null -ne $bookId) {
        $cleanup = Get-Page ('/Books/Delete/' + $bookId)
        if ($cleanup.StatusCode -eq 200 -and $cleanup.Content.Contains($title)) {
            $null = Post-Form ('/Books/Delete/' + $bookId) @{ '__RequestVerificationToken' = Get-Token $cleanup }
        }
    }
}
