
module GitStatsReder

open System.Diagnostics

let readGitStats repoPath =
    let startInfo = ProcessStartInfo ()
    startInfo.FileName <- "git.exe"
    startInfo.WorkingDirectory <- repoPath
    startInfo.Arguments <- "log --stat"
    startInfo.UseShellExecute <- false
    startInfo.RedirectStandardOutput <- true
    startInfo.CreateNoWindow <- true
    seq {
        let proc = Process.Start(startInfo);
        while not proc.StandardOutput.EndOfStream do
            yield proc.StandardOutput.ReadLine()
    }


