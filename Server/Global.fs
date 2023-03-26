[<AutoOpen>]
module Global

type List<'a> = list<'a>
type Seq<'a> = seq<'a>
type map<'k, 'v when 'k: comparison> = Map<'k, 'v>
type set<'a when 'a: comparison> = Set<'a>
