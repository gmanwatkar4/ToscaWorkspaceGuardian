Param(
	[string]$root = "$(Resolve-Path ..).Path"
)

Write-Output "Running add_file_headers_and_docs on $root"

$files = Get-ChildItem -Path $root -Recurse -Include *.cs -File | Where-Object { $_.FullName -notmatch "\\bin\\|\\obj\\|\\.git\\" }

foreach ($file in $files) {
	$text = Get-Content -Raw -Path $file.FullName

	$modified = $false

	# Add file header if missing
	if ($text -notmatch "<copyright" -and $text -notmatch "// <copyright") {
		$header = @'
<!--
// <copyright file="{FILENAME}" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
-->

'@
		$header = $header -replace '\{FILENAME\}',$file.Name
		$text = $header + $text
		$modified = $true
	}

	# Add minimal XML doc for public classes/interfaces/structs that don't have one
	$pattern = '(?m)^(\s*)(public\s+(?:partial\s+)?(?:class|interface|struct)\s+([A-Za-z0-9_]+))'
	$text = [System.Text.RegularExpressions.Regex]::Replace($text, $pattern, {
		param($m)
		$leading = $m.Groups[1].Value
		$decl = $m.Groups[2].Value
		$name = $m.Groups[3].Value
		# Check if there's XML doc immediately before
		$before = $m.Index - 1
		$hasDoc = $false
		if ($before -ge 0) {
			$start = [Math]::Max(0, $before - 200)
			$len = [Math]::Min(200, $before - $start + 1)
			$snippet = $text.Substring($start, $len)
			if ($snippet -match "///\s*<summary>") { $hasDoc = $true }
		}
		if ($hasDoc) { return $m.Value }
		$doc = "$leading/// <summary>`n$leading/// TODO: Describe $name.`n$leading/// </summary>`n"
		$script:modified = $true
		return $doc + $m.Value
	})

	if ($modified -or $script:modified) {
		Set-Content -Path $file.FullName -Value $text -Encoding UTF8
		Write-Output "Modified: $($file.FullName)"
		$script:modified = $false
	}
}

Write-Output "Done"
