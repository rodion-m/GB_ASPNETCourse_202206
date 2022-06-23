namespace Lesson.AsyncFeatures;

public class ValueTaskExample
{
    private string? _top250;

    public async ValueTask<string> GetTop250()
    {
        if (_top250 is not null)
        {
            return _top250;
        }

        _top250 = await File.ReadAllTextAsync("top250.txt");
        return _top250;
    }
    
    public  ValueTask<string> GetTop250Superfluously()
    {
        if (_top250 is not null)
        {
            return ValueTask.FromResult(_top250);
        }

        return new ValueTask<string>(File.ReadAllTextAsync("top250.txt")
            .ContinueWith(task => _top250 = task.Result));
    }
    //https://sharplab.io/#v2:D4AQTAjAsAUCAMACEEB0BJA8gblg5aAKgBYBOApgIYAmAlgHYDmucSKArC3gMzJiIBhRAG9YicYjESADqVoA3SgBdyyACyIAQgFclSgPb0A+gIA2tAMYBrABT6ARgCtyFpYgDO5etXKkAlFLiojASoQD0YQDqlLRKmPTkAMouNn6oAOLkSgCCAO4xKqSpGVkASuTu2qZKqdiIEYAsIICsIA2A3CCAfCCAXCCIgIIgrYDCIIDiIIAcIIBCID2A8iCIgOwgzT2AMiCBodGx8UkpfiyhEhGA/CCADCD90/tL4WEChkoM2sq0hgBcp+KKpIgqAB5uALyIAETu+gAtkpiAxGL8thIAL6wWEhGRyRQqAhIIwGaRgdjwSHiJayBTKVQgAAciAAapRTNpyCAAGyIFZxBLJCypJ6IL4APmQAE5kLTUAARcimSgATxshFogKS0ko9FQADFSECWYZqO4bBA/JslksQLxyZTqXSADwoeDczJKQj6DFYtnwoLsxC0ABmiBsaLtmKQtHciHo+jc9CqpgCTu2wW2MeQAHYjVSaQLlUDypVql70b7dZHQjCYC6QAmErlEybaeaIJabIraKZyKhyjRsqZTIRyJ9su4xfRWb9s1jUEpPr8I7HtqgLvQrqHyJFYsQbEpKO4rBzud77Ugfiu102KlUlDqcYgC1CgA===
}