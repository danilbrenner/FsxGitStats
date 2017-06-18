module CommitInfoModule

open System

type NumberOfFilesChanged = NumberOfFilesChanged of int
type NumberOfInsertions = NumberOfInsertions of int
type NumberOfDeletions = NumberOfDeletions of int

type CommitInfo = {
    Hash: string
    Date: DateTime
    Author: string
    FilesChanged: NumberOfFilesChanged
    Insertions: NumberOfInsertions
    Deletions: NumberOfDeletions
}

let private parseDt dt = DateTime.ParseExact(dt, "ddd MMM d HH:mm:ss yyyy zzz", Globalization.CultureInfo.InvariantCulture)

let private substrToInt toRem (str:string) = int <| str.Substring(0, (str.Length - toRem)).Trim()

let private foldStatsStr commit (str:string) =
    match str with
    | itm when itm.EndsWith("files changed") -> { commit with FilesChanged = NumberOfFilesChanged (substrToInt 14 itm) }
    | itm when itm.EndsWith("file changed") -> { commit with FilesChanged = NumberOfFilesChanged (substrToInt 13 itm) }
    | itm when itm.EndsWith("insertions(+)") -> { commit with Insertions = NumberOfInsertions (substrToInt 14 itm) }
    | itm when itm.EndsWith("insertion(+)") -> { commit with Insertions = NumberOfInsertions (substrToInt 13 itm) }
    | itm when itm.EndsWith("deletions(-)") -> { commit with Deletions = NumberOfDeletions (substrToInt 13 itm) }
    | itm when itm.EndsWith("deletion(-)") -> { commit with Deletions = NumberOfDeletions (substrToInt 12 itm) }
    | _ -> commit

let private parseStatsStr commit ( str:string) =
    str
    |> (fun i -> i.Split(','))
    |> Array.map (fun i -> i.Trim())
    |> Array.fold foldStatsStr commit

let private foldCommitInfo commit (str:string) =
    match str with
    | itm when itm.StartsWith("commit") -> { commit with Hash = itm.Substring(7) }
    | itm when itm.StartsWith("Author:") -> { commit with Author = itm.Substring(8) }
    | itm when itm.StartsWith("Date:") -> { commit with Date = (parseDt (itm.Substring(8))) }
    | itm when
        (itm.Contains("files changed")
        || itm.Contains("file changed"))
        && (itm.Contains("insertion")
        || itm.Contains("deletion"))
            -> parseStatsStr commit itm
    | _ -> commit

let processCommitInfo sq =
    sq
    |> Seq.map (fun (_,str) -> str)
    |> Seq.fold foldCommitInfo {
                Hash = ""
                Date = DateTime.MinValue
                Author = ""
                FilesChanged = NumberOfFilesChanged 0
                Insertions = NumberOfInsertions 0
                Deletions = NumberOfDeletions 0
            }