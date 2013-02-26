﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lucinq.Interfaces;
using Lucinq.Sitecore.Constants;
using Lucinq.Sitecore.Interfaces;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Lucinq.Sitecore.Querying
{
	public class SitecoreSearchResult : ISearchResult
	{
		#region [ Fields ]

		#endregion

		#region [ Constructors ]

		public SitecoreSearchResult(ILuceneSearchResult searchResult, IDatabaseHelper databaseHelper)
		{
			DatabaseHelper = databaseHelper;
			LuceneSearchResult = searchResult;
		}

		#endregion

		#region [ Properties ]

		public IDatabaseHelper DatabaseHelper { get; private set; }

		public ILuceneSearchResult LuceneSearchResult { get; private set; }

		public int TotalHits { get { return LuceneSearchResult.TotalHits; } }

		public long ElapsedTimeMs { get; set; }

		#endregion

		#region [ Methods ]

		/// <summary>
		/// Gets a list of items for the documents
		/// </summary>
		/// <returns></returns>
		public SitecoreItemResult GetPagedItems(int start, int end)
		{
			List<Item> items = new List<Item>();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			LuceneSearchResult.GetPagedDocuments(start, end).ForEach(
				document =>
					{
						string itemShortId = document.GetValues(SitecoreFields.Id).FirstOrDefault();
						if (String.IsNullOrEmpty(itemShortId))
						{
							return;
						}
						ID itemId = new ID(itemShortId);
						Item item = DatabaseHelper.GetItem(itemId);
						items.Add(item);
					}
				);
			stopwatch.Stop();
			return new SitecoreItemResult(items) { ElapsedTimeMs = stopwatch.ElapsedMilliseconds };
		}

		#endregion
	}
}
