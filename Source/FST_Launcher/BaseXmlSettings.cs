//==============================================================
// Forex Strategy Trader
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace FST_Launcher
{
    public class BaseXmlSettings
    {
        private PropertyInfo[] propertyInfos;

        public string PathSettings { get; set; }

        private IEnumerable<PropertyInfo> GetPropertiesInfo(object settings)
        {
            return propertyInfos ??
                   (propertyInfos = settings.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }

        protected void BaseLoadSettings(object settings)
        {
            XmlDocument xmlDocument = LoadXmlDocument(PathSettings);
            if (xmlDocument == null) return;

            IEnumerable<PropertyInfo> infos = GetPropertiesInfo(settings);
            foreach (PropertyInfo info in infos)
            {
                string nodeName = @"Settings/" + info.Name;
                Type type = info.PropertyType;
                object value = ParseNode(xmlDocument, nodeName, info.GetValue(settings, null), type);
                info.SetValue(settings, value, null);
            }
        }

        protected void BaseSaveSettings(object settings)
        {
            IEnumerable<PropertyInfo> infos = GetPropertiesInfo(settings);

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<Settings>");
            foreach (PropertyInfo info in infos)
            {
                sb.Append(string.Format("\t<{0}>", info.Name));
                sb.Append(ValueToString(info.GetValue(settings, null), info.PropertyType));
                sb.AppendLine(string.Format("</{0}>", info.Name));
            }
            sb.AppendLine("</Settings>");

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sb.ToString());

            SaveXmlDocument(xmlDocument, PathSettings);
        }

        private XmlDocument LoadXmlDocument(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(path);
                    return xmlDocument;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        private void SaveXmlDocument(XmlDocument xmlDocument, string path)
        {
            try
            {
                xmlDocument.Save(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string ValueToString(object value, Type type)
        {
            if (type == typeof (double))
                return ((double) value).ToString(CultureInfo.InvariantCulture);
            if (type == typeof (DateTime))
                return ((DateTime) value).ToString(CultureInfo.InvariantCulture);
            if (type == typeof (Color))
                return ColorTranslator.ToHtml((Color) value);

            return value.ToString();
        }

        private object ParseNode(XmlDocument doc, string node, object defaultValue, Type type)
        {
            try
            {
                XmlNode xmlNode = doc.SelectSingleNode(node);
                if (xmlNode == null)
                    return defaultValue;

                string text = xmlNode.InnerText;
                if (string.IsNullOrEmpty(text))
                    return defaultValue;

                return ParseType(text, type)
                       ?? defaultValue;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return defaultValue;
            }
        }

        private object ParseType(string text, Type type)
        {
            switch (type.Name)
            {
                case "Char":
                    return Convert.ChangeType(Char.Parse(text), typeof (Char));
                case "String":
                    return Convert.ChangeType(text, typeof (String));
                case "Boolean":
                    return Convert.ChangeType(Boolean.Parse(text), typeof (Boolean));
                case "Int16":
                    return Convert.ChangeType(Int16.Parse(text), typeof (Int16));
                case "Int32":
                    return Convert.ChangeType(Int32.Parse(text), typeof (Int32));
                case "Int64":
                    return Convert.ChangeType(Int64.Parse(text), typeof (Int64));
                case "Decimal":
                    return Convert.ChangeType(Decimal.Parse(text), typeof (Decimal));
                case "Double":
                    return Convert.ChangeType(Double.Parse(text, CultureInfo.InvariantCulture), typeof (Double));
                case "DateTime":
                    return Convert.ChangeType(DateTime.Parse(text, CultureInfo.InvariantCulture), typeof (DateTime));
                case "Color":
                    return Convert.ChangeType(ColorTranslator.FromHtml(text), typeof (Color));
            }

            return Convert.ChangeType(text, typeof (Object), CultureInfo.InvariantCulture);
        }
    }
}