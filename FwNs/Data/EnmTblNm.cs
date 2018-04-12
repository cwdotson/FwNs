namespace FwNs.Data
{
    using System;

    public enum EnmTblNm
    {
        [AttrbTblNm("NonXst", 0)]
        NonXst = -1,
        [AttrbTblNm("Wyth", 0)]
        Wyth = 0,
        [AttrbTblNm("DbRoot", 0x3e7)]
        DbRoot = 2,
        [AttrbTblNm("RowRef", 2, "IdDbRoot")]
        RowRef = 4,
        [AttrbTblNm("Drv", 2, "IdDbRoot")]
        Drv = 110,
        [AttrbTblNm("Ctnr", 110, "IdD, Itm")]
        Ctnr = 120,
        [AttrbTblNm("Itm", 120, "IdC, Part")]
        Itm = 130,
        [AttrbTblNm("Part", 130, "Idp")]
        Part = 140,
        [AttrbTblNm("Wrd", 2, "IdDbRoot")]
        Wrd = 200,
        [AttrbTblNm("WrdX", 2, "IdDbRoot")]
        WrdX = 0xca,
        [AttrbTblNm("CTyp", 2, "IdDbRoot, FTyp")]
        CTyp = 300,
        [AttrbTblNm("FTyp", 300, "Idp")]
        FTyp = 310,
        [AttrbTblNm("Mchn", 2, "IdDbRoot, Usr")]
        Mchn = 400,
        [AttrbTblNm("Usr", 400, "IdM")]
        Usr = 410,
        [AttrbTblNm("Grp", 2, "IdDbRoot")]
        Grp = 500,
        [AttrbTblNm("Sqnc", 2, "IdDbRoot")]
        Sqnc = 600,
        [AttrbTblNm("SqncInt", 2, "IdDbRoot")]
        SqncInt = 610,
        [AttrbTblNm("SqncStr", 2, "IdDbRoot")]
        SqncStr = 620,
        [AttrbTblNm("Othr", 2, "IdDbRoot")]
        Othr = 700,
        [AttrbTblNm("Ctxt", -1, "IdDbRoot")]
        DbCtxt = 0x3e7,
        [AttrbTblNm("RowTag", 2, "IdDbRoot")]
        RowTag = 0x12,
        [AttrbTblNm("Tag", 2, "IdDbRoot")]
        Tag = 0x13,
        [AttrbTblNm("TblId", 2, "IdDbRoot")]
        TblId = 20,
        [AttrbTblNm("EnmTyps", 2, "IdDbRoot")]
        EnmTyp = 0x16
    }
}

