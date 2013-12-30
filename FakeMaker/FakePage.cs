﻿using System;
using System.Collections.Generic;
using EPiServer.Core;

namespace EPiFakeMaker
{
	public class FakePage
	{
		public virtual PageData Page { get; private set; }

		private readonly IList<FakePage> _children;

		private static readonly Random Randomizer = new Random();

		public virtual IList<FakePage> Children { get { return _children; } }

		private FakePage()
		{
			_children = new List<FakePage>();
		}

		public static FakePage Create(string pageName)
		{
			return Create<PageData>(pageName);
		}

		public static FakePage Create<T>(string pageName) where T : PageData, new()
		{
			var fake = new FakePage {Page = new T()};

			fake.Page.Property["PageName"] = new PropertyString(pageName);

			fake.WithReferenceId(Randomizer.Next(10, 1000));

			fake.VisibleInMenu();

			return fake;
		}

		public virtual FakePage ChildOf(FakePage parent)
		{
			parent.Children.Add(this);

			Page.Property["PageParentLink"] = new PropertyPageReference(parent.Page.ContentLink);

			return this;
		}

		public virtual FakePage PublishedOn(DateTime publishDate)
		{
			 PublishedOn(publishDate, null);

			return this;
		}

		public virtual FakePage PublishedOn(DateTime publishDate, DateTime? stopPublishDate)
		{
			Page.Property["PageStartPublish"] = new PropertyDate(publishDate);

			WorkStatus(VersionStatus.Published);

			StopPublishOn(stopPublishDate.HasValue ? stopPublishDate.Value : publishDate.AddYears(1));

			return this;
		}

		public virtual FakePage VisibleInMenu()
		{
			return SetMenuVisibility(true);
		}

		public virtual FakePage HiddenFromMenu()
		{
			return SetMenuVisibility(false);
		}

		public virtual FakePage SetMenuVisibility(bool isVisible)
		{
			Page.Property["PageVisibleInMenu"] = new PropertyBoolean(isVisible);

			return this;
		}

		public virtual FakePage WithReferenceId(int referenceId)
		{
			Page.Property["PageLink"] = new PropertyPageReference(new PageReference(referenceId));

			return this;
		}

		public virtual FakePage StopPublishOn(DateTime stopPublishDate)
		{
			Page.Property["PageStopPublish"] = new PropertyDate(stopPublishDate);

			return this;
		}

		public virtual FakePage WorkStatus(VersionStatus status)
		{
			Page.Property["PageWorkStatus"] = new PropertyNumber((int)status);

			return this;
		}

		public virtual T To<T>() where T : PageData
		{
			return Page as T;
		}
	}
}
