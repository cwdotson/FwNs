namespace FwNs.Core.LC.cResults
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cTables;
    using System;

    public sealed class ResultMetaData
    {
        public const int ResultMetadata = 1;
        public const int SimpleResultMetadata = 2;
        public const int UpdateResultMetadata = 3;
        public const int ParamMetadata = 4;
        public const int GeneratedIndexMetadata = 5;
        public const int GeneratedNameMetadata = 6;
        public static ResultMetaData EmptyResultMetaData = NewResultMetaData(0);
        public static ResultMetaData EmptyParamMetaData = NewParameterMetaData(0);
        private int _columnCount;
        private int _extendedColumnCount;
        public int[] ColIndexes;
        public string[] ColumnLabels;
        public SqlType[] ColumnTypes;
        public ColumnBase[] columns;
        public byte[] ParamModes;
        public byte[] ParamNullable;
        public bool IsNamedParameters;

        private ResultMetaData(int type)
        {
        }

        public int GetColumnCount()
        {
            return this._columnCount;
        }

        public int GetExtendedColumnCount()
        {
            return this._extendedColumnCount;
        }

        public int[] GetGeneratedColumnIndexes()
        {
            return this.ColIndexes;
        }

        public string[] GetGeneratedColumnNames()
        {
            return this.ColumnLabels;
        }

        public ResultMetaData GetNewMetaData(int[] columnMap)
        {
            ResultMetaData data = NewResultMetaData(columnMap.Length);
            ArrayUtil.ProjectRow(this.ColumnLabels, columnMap, data.ColumnLabels);
            ArrayUtil.ProjectRow(this.ColumnTypes, columnMap, data.ColumnTypes);
            ArrayUtil.ProjectRow(this.columns, columnMap, data.columns);
            return data;
        }

        public SqlType[] GetParameterTypes()
        {
            return this.ColumnTypes;
        }

        public static ResultMetaData NewGeneratedColumnsMetaData(int[] columnIndexes, string[] columnNames)
        {
            if (columnIndexes != null)
            {
                ResultMetaData data2 = new ResultMetaData(5) {
                    _columnCount = columnIndexes.Length,
                    _extendedColumnCount = columnIndexes.Length,
                    ColIndexes = new int[columnIndexes.Length]
                };
                for (int i = 0; i < columnIndexes.Length; i++)
                {
                    data2.ColIndexes[i] = columnIndexes[i] - 1;
                }
                return data2;
            }
            if (columnNames != null)
            {
                return new ResultMetaData(6) { 
                    ColumnLabels = new string[columnNames.Length],
                    _columnCount = columnNames.Length,
                    _extendedColumnCount = columnNames.Length,
                    ColumnLabels = columnNames
                };
            }
            return null;
        }

        public static ResultMetaData NewParameterMetaData(int colCount)
        {
            return new ResultMetaData(4) { 
                ColumnTypes = new SqlType[colCount],
                ColumnLabels = new string[colCount],
                ParamModes = new byte[colCount],
                ParamNullable = new byte[colCount],
                _columnCount = colCount,
                _extendedColumnCount = colCount
            };
        }

        public static ResultMetaData NewResultMetaData(int colCount)
        {
            return NewResultMetaData(new SqlType[colCount], null, colCount, colCount);
        }

        public static ResultMetaData NewResultMetaData(SqlType[] types, int[] baseColumnIndexes, int colCount, int extColCount)
        {
            return new ResultMetaData(1) { 
                ColumnLabels = new string[colCount],
                columns = new ColumnBase[colCount],
                ColumnTypes = types,
                ColIndexes = baseColumnIndexes,
                _columnCount = colCount,
                _extendedColumnCount = extColCount
            };
        }

        public static ResultMetaData NewSimpleResultMetaData(SqlType[] types)
        {
            return new ResultMetaData(2) { 
                ColumnTypes = types,
                _columnCount = types.Length,
                _extendedColumnCount = types.Length
            };
        }

        public static ResultMetaData NewUpdateResultMetaData(SqlType[] types)
        {
            ResultMetaData data = new ResultMetaData(3) {
                ColumnTypes = new SqlType[types.Length],
                _columnCount = types.Length,
                _extendedColumnCount = types.Length
            };
            Array.Copy(types, data.ColumnTypes, types.Length);
            return data;
        }

        public void PrepareData()
        {
            if (this.columns != null)
            {
                for (int i = 0; i < this._columnCount; i++)
                {
                    if (this.ColumnTypes[i] == null)
                    {
                        this.ColumnTypes[i] = this.columns[i].GetDataType();
                    }
                }
            }
        }

        public void ResetExtendedColumnCount()
        {
            this._extendedColumnCount = this._columnCount;
        }
    }
}

