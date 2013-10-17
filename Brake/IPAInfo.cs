using System;
using System.Collections.Generic;

namespace Brake
{
	public class IPAInfo
	{
		public string AppName;
		public string AppVersion;
		public string AppBundle;
		public string Location;

	}
	public class Container
	{
		public List<IPAInfo> Items;
		public Container()
		{
			Items = new List<IPAInfo>();
		}
	}

}

