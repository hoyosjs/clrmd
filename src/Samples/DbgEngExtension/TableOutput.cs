﻿using Microsoft.Diagnostics.Runtime.Utilities.DbgEng;
using System.Text;

namespace DbgEngExtension
{
    public class TableOutput
    {
        private readonly char _spacing = ' ';
        public string Divider { get; set; } = " ";
        public bool AlignLeft { get; set; } = false;

        private readonly (int width, string format)[] _formats;

        public TableOutput(params (int width, string format)[] columns)
        {
            _formats = columns.ToArray();
        }

        public void WriteRow(params object?[] columns)
        {
            StringBuilder sb = new(Divider.Length * columns.Length + _formats.Sum(c => Math.Abs(c.width)) + 32);

            for (int i = 0; i < columns.Length; i++)
            {
                if (i != 0)
                    sb.Append(Divider);

                (int width, string format) = i < _formats.Length ? _formats[i] : default;

                string? value;
                if (string.IsNullOrWhiteSpace(format))
                    value = columns[i]?.ToString();
                else
                    value = Format(columns[i], format);

                AddValue(_spacing, sb, width, value ?? "");
            }

            Console.WriteLine(sb);
        }

        public void WriteRowWithSpacing(char spacing, params object?[] columns)
        {
            StringBuilder sb = new(columns.Length + _formats.Sum(c => Math.Abs(c.width)));

            for (int i = 0; i < columns.Length; i++)
            {
                if (i != 0)
                    sb.Append(spacing, Divider.Length);

                (int width, string format) = i < _formats.Length ? _formats[i] : default;

                string? value;
                if (string.IsNullOrWhiteSpace(format))
                    value = columns[i]?.ToString();
                else
                    value = Format(columns[i], format);

                AddValue(spacing, sb, width, value ?? "");
            }

            Console.WriteLine(sb);
        }


        public void WriteSpacer(char spacer)
        {
            Console.WriteLine(new string(spacer, Divider.Length * (_formats.Length - 1) + _formats.Sum(c => Math.Abs(c.width))));
        }

        private void AddValue(char spacing, StringBuilder sb, int width, string value)
        {
            bool leftAlign = AlignLeft ? width > 0 : width < 0;
            width = Math.Abs(width);

            if (width == 0)
            {
                sb.Append(value);
            }
            else if (value.Length > width)
            {
                if (width <= 3)
                {
                    sb.Append(value, 0, width);
                }
                else if (leftAlign)
                {
                    value = value.Substring(0, width - 3);
                    sb.Append(value);
                    sb.Append("...");
                }
                else
                {
                    value = value[^(width - 3)..];
                    sb.Append("...");
                    sb.Append(value);
                }

            }
            else if (leftAlign)
            {
                sb.Append(value.PadRight(width, spacing));
            }
            else
            {
                sb.Append(value.PadLeft(width, spacing));
            }
        }

        private static string? Format(object? obj, string format)
        {
            if (obj is null)
                return null;

            if (obj is Enum)
                return obj.ToString();

            return obj switch
            {
                nint ni => ni.ToString(format),
                ulong ul => ul.ToString(format),
                long l => l.ToString(format),
                uint ui => ui.ToString(format),
                int i => i.ToString(format),
                string s => s,
                StringBuilder sb => sb.ToString(),
                _ => throw new NotImplementedException(obj.GetType().ToString()),
            };
        }
    }
}
