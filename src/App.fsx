
#load "GitStatsReder.fsx"

GitStatsReder.readGitStats """Path"""
|> Seq.iter (fun i -> printfn "%s" i)