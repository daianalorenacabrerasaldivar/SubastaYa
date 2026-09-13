param(
    [string]$BaseUrl = "http://localhost:5073",
    [int]$AuctionId = 1,
    [int]$CompradorId = 3,
    [decimal]$Monto = 60000
)

$body = @{ compradorId = $CompradorId; monto = $Monto } | ConvertTo-Json
$url = "$BaseUrl/api/v1/auctions/$AuctionId/bids"

$jobs = 1..2 | ForEach-Object {
    Start-Job -ScriptBlock {
        param($u, $b)
        try {
            $r = Invoke-WebRequest -Uri $u -Method Post -Body $b -ContentType "application/json" -UseBasicParsing
            "HTTP $($r.StatusCode)"
        }
        catch {
            "HTTP $($_.Exception.Response.StatusCode.value__) $($_.ErrorDetails.Message)"
        }
    } -ArgumentList $url, $body
}

$jobs | Wait-Job | Receive-Job
$jobs | Remove-Job
