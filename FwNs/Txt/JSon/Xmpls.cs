namespace FwNs.Txt.JSon
{
    using JsonFx.Json;
    using System;

    public static class Xmpls
    {
        public static object Test000()
        {
            return null;
        }

        public static object Test000(object obj)
        {
            new { 
                kee = "",
                val = ""
            };
            string[] textArray1 = new string[] { "Statk", "Xxcld", "Enms", "Cmplx", "Cstm", "Hlprs", "Xmpls", "XxRzx" };
            var type = new {
                kee = "Xxcld",
                val = textArray1
            };
            string[] textArray2 = new string[] { "Xxcld" };
            string[] textArray3 = new string[] { "Xxcld" };
            string[] textArray4 = new string[] { "Xxcld" };
            string[] textArray5 = new string[] { "Stngs", "Xxcld" };
            string[] textArray6 = new string[] { "Statk", "Xxcld", "Enms", "Cmplx", "Cstm", "Hlprs", "Xmpls", "XxRzx" };
            return new JsonWriter().Write(new { fwns = new { 
                Code = new { fldrz = type },
                Core = new { 
                    Cmplx = new { 
                        kee = "Cmplx",
                        val = textArray2
                    },
                    Enms = new { 
                        kee = "Enms",
                        val = textArray3
                    },
                    Hlprs = new { 
                        kee = "Hlprs",
                        val = textArray4
                    },
                    Statk = new { 
                        kee = "Statk",
                        val = textArray5
                    },
                    Cstm = new { 
                        kee = "Cstm",
                        val = textArray6
                    }
                },
                Data = new { fldrz = type },
                Draw = new { fldrz = type },
                FSys = new { fldrz = type },
                Geo = new { fldrz = type },
                Gui = new { fldrz = type },
                JSon = new { fldrz = type },
                Misc = new { fldrz = type },
                Offc = new { fldrz = type },
                Task = new { fldrz = type },
                Txt = new { fldrz = type },
                Typs = new { fldrz = type },
                Web = new { fldrz = type },
                Xml = new { fldrz = type },
                Xprsn = new { fldrz = type }
            } });
        }

        public static object Test000(object obj0, object obj1)
        {
            return null;
        }
    }
}

