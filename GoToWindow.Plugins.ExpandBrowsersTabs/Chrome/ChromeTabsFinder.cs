﻿using System;
using System.Collections.Generic;
using System.Windows.Automation;
using GoToWindow.Plugins.ExpandBrowsersTabs.Contracts;

namespace GoToWindow.Plugins.ExpandBrowsersTabs.Chrome
{
	/// <remarks>
	/// Thanks to CoenraadS: https://github.com/CoenraadS/Chrome-Tab-Switcher
	/// </remarks>
	public class ChromeTabsFinder : ITabsFinder
	{
		public IEnumerable<ITab> GetTabsOfWindow(IntPtr hWnd)
		{
			var parent = AutomationElement.FromHandle(hWnd);

			var mainElement = parent.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));

			if (mainElement == null)
				yield break;
			
			var tabBarElement = mainElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "tab"));

			var tabElements = tabBarElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "tab item"));

			for (var tabIndex = 0; tabIndex < tabElements.Count; tabIndex++)
			{
				yield return new ChromeTab(tabElements[tabIndex].Current.Name, tabIndex + 1);
			}
		}
	}
}