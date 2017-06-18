#load "GitStatsReder.fsx"
#load "CommitInfoModule.fsx"
#load "../packages/FSharp.Charting/FSharp.Charting.fsx"

open FSharp.Charting

let repoPath = """---"""

let splitSeq fn sq =
    let mutable inc = 0
    sq
    |> Seq.map (fun i ->
        inc <- if fn(i) then inc+1 else inc
        (inc, i)
    )
    |> Seq.groupBy fst
    |> Seq.map (fun (_, ix) -> ix)

let sq =
    GitStatsReder.readGitStats repoPath
    |> splitSeq (fun l -> l.StartsWith("commit "))
    |> Seq.map CommitInfoModule.processCommitInfo

sq
|> Seq.countBy (fun i -> i.Date.Date)
|> Seq.rev
|> (fun i -> Chart.Line (i, Name = "Number of commits"))
//---