using System;
using System.Collections.Generic;

namespace BinaryStudio.PlatformUI.Shell
    {
    internal class WindowProfileValidator
        {
        public void Validate(WindowProfile profile)
            {
            if (profile.Children.Count >= 20000)
                throw new InvalidOperationException("WindowProfile contains a Children collection which has exceeded capacity.");
            var mainSites = new List<MainSite>();
            for (var index = 0; index < profile.Children.Count; ++index)
                {
                var child = profile.Children[index];
                if (child is MainSite)
                    mainSites.Add(child as MainSite);
                if (!(!(child is ViewGroup) ? ValidateElement(child) : ValidateGroup(child as ViewGroup)))
                    {
                    profile.Children.Remove(child);
                    --index;
                    }
                }
            if (mainSites.Count == 0)
                {
                var mainSite = MainSite.Create();
                mainSite.Child = WindowProfile.CreateDefaultViewSiteContent();
                profile.Children.Add(mainSite);
                }
            else
                {
                if (mainSites.Count > 1)
                    DeleteExtraMainSites(mainSites, profile);
                foreach (var site in mainSites)
                    PostValidation(site);
                }
            }

        private Boolean ValidateGroup(ViewGroup group)
            {
            var flag = false;
            if (group.Children.Count >= 20000)
                throw new InvalidOperationException("ViewGroup contains a Children collection which has exceeded capacity.");
            group.TryCollapse();
            if (group.Parent != null || group is ViewSite)
                {
                flag = ValidateElement(@group);
                if (flag)
                    {
                    for (var index = 0; index < group.Children.Count; ++index)
                        {
                        var child = group.Children[index];
                        if (child is AutoHideGroup)
                            ((ViewGroup)child).SelectedElement = null;
                        if (!(!(child is ViewGroup) ? ValidateElement(child) : ValidateGroup(child as ViewGroup)))
                            {
                            group.Children.Remove(child);
                            --index;
                            }
                        }
                    }
                }
            return flag;
            }

        private Boolean ValidateElement(ViewElement element)
            {
            if (element.AutoHideWidth <= 0.0)
                element.AutoHideWidth = 200.0;
            if (element.AutoHideHeight <= 0.0)
                element.AutoHideHeight = 200.0;
            var site1 = element as MainSite;
            if (site1 != null)
                return ValidateElement(site1);
            var site2 = element as FloatSite;
            if (site2 != null)
                return ValidateElement(site2);
            var container = element as DocumentGroupContainer;
            if (container != null)
                return ValidateElement(container);
            return true;
            }

        private Boolean ValidateElement(MainSite site)
            {
            var flag = true;
            if (site.Parent != null)
                flag = false;
            return flag;
            }

        private Boolean ValidateElement(FloatSite site)
            {
            var flag = true;
            if (site.Children.Count == 0)
                flag = false;
            if (site.Parent != null)
                flag = false;
            return flag;
            }

        private Boolean ValidateElement(DocumentGroupContainer container)
            {
            var flag = true;
            if (container.Children.Count == 0)
                container.Children.Add(DocumentGroup.Create());
            return flag;
            }

        private void PostValidation(MainSite site)
            {
            if (site.Find<DocumentGroup>(false) != null)
                return;
            site.Child = WindowProfile.CreateDefaultViewSiteContent();
            }

        private void DeleteExtraMainSites(List<MainSite> mainSites, WindowProfile profile)
            {
            var mainSite1 = (MainSite)null;
            var num = 0;
            foreach (var mainSite2 in mainSites)
                {
                var viewElementList = new List<ViewElement>(mainSite2.FindAll(v => true));
                if (mainSite1 == null || viewElementList.Count > num)
                    {
                    mainSite1 = mainSite2;
                    num = viewElementList.Count;
                    }
                }
            mainSites.Remove(mainSite1);
            foreach (var mainSite2 in mainSites)
                profile.Children.Remove(mainSite2);
            mainSites.Clear();
            mainSites.Add(mainSite1);
            }
        }
    }