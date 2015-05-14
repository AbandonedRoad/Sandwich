using System;
using UnityEngine;
using System.Xml;
using Blocks;
using System.Collections.Generic;

namespace Singleton
{
	public class XMLSingleton
	{
		private static XMLSingleton _instance;

		public Dictionary<string, ObjectInfo> ObjectInfos = new Dictionary<string, ObjectInfo>();
		
		/// <summary>
		/// Gets instance
		/// </summary>
		public static XMLSingleton Instance
		{
			get 
			{
				if (_instance == null) 
				{
					_instance = new XMLSingleton();
					_instance.ReadItems();
				}
				
				return _instance;
			}
		}

		/// <summary>
		/// Reads the items.
		/// </summary>
		public void ReadItems()
		{
			// ------------------------
			// Forenames
			// ------------------------
            XmlNodeList items = GetItems("XML/ObjectInfo", "/ObjectInfos", "ObjectInfo");
			
			// Load Items
			foreach (XmlNode item in items) 
			{
				ObjectInfo objectInfo = new ObjectInfo(item["Key"].InnerText, item["ReduceSide"].InnerText);
				ObjectInfos.Add(objectInfo.Key, objectInfo);
			}
		}		

		/// <summary>
		/// Gets the items.
		/// </summary>
		/// <returns>The items.</returns>
		/// <param name="path">Path.</param>
		/// <param name="header">Header.</param>
		/// <param name="child">Child.</param>
		private XmlNodeList GetItems(string path, string header, string child)
		{
			TextAsset bindata= Resources.Load(path) as TextAsset;
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(bindata.text);
			
			XmlNode itemGroup = doc.SelectSingleNode(header);
			XmlNodeList items = itemGroup.SelectNodes(child);
			
			return items;
		}
	}
}