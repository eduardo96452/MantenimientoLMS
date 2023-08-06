/*
*  
*  Copyright (C) 2010-2011 University College Ghent.
*  
*  For a full list of contributors, see "credits.txt".
*  
*  This file is part of LMS Desktop Assistant.
*  
*  LMS Desktop Assistant is free software: you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*  
*  LMS Desktop Assistant is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*  
*  You should have received a copy of the GNU General Public License
*  along with LMS Desktop Assistant. If not, see <http://www.gnu.org/licenses/>.
*  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using lmsda.domain;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     
    ///     This class can be used for displaying tooltips over disabled controls.
    ///     
    ///     How to call: 
    ///         -> On a MouseMove-event: call method "updateTooltips(container.GetChildAtPoint(e.Location));"
    ///     
    ///     Attention: if a control is located inside a container, then that container should also implement the MouseMove-event!
    /// </summary>
    /// <remarks>
    ///     Last updated on 13/08/2010 by Gianni Van Hoecke
    ///      -> added timer to auto-hide the tooltip.
    /// 
    ///     Updated on 13/08/2010 by Gianni Van Hoecke
    ///      -> Workaround for bug. (See explanation at the second constructor)
    /// </remarks>
    class TooltipChecker
    {
        private ToolTip tooltip;
        private Boolean isTooltipShown;
        private List<Control> controls;
        private List<String> controlTooltips;
        private int lastControl = -1;
        public Boolean showTooltips {get; set;}
        private ToolTip oldTooltip;
        private String oldTooltipString;
        private Timer timer;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <remarks>
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public TooltipChecker()
        {
            this.oldTooltip = new ToolTip();
            this.oldTooltipString = String.Empty;
            this.tooltip = new ToolTip();
            this.isTooltipShown = false;
            this.controls = new List<Control>();
            this.controlTooltips = new List<String>();
            this.showTooltips = true;
            this.timer = new Timer();
            this.setTimerInterval(5000);
            this.timer.Tick += new EventHandler(Timer_Tick);
        }

        /// <summary>
        ///     Constructor with pre-defined ToolTip object. We MUST pass the original ToolTip object 
        ///     because of a strange bug in Microsofts' code. 
        ///     When we mouse-over a control which is disabled, our tooltip appears.
        ///     When we then mouse-over our tooltip, the original tooltip will be displayed. 
        ///     Why? most likely a bug in Microsofts' code.
        /// </summary>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public TooltipChecker(ToolTip tt)
        {
            this.oldTooltip = new ToolTip();
            this.oldTooltipString = String.Empty;
            this.tooltip = new ToolTip();
            this.isTooltipShown = false;
            this.controls = new List<Control>();
            this.controlTooltips = new List<String>();
            this.showTooltips = true;
            this.oldTooltip = tt;
            this.oldTooltipString = String.Empty;
            this.timer = new Timer();
            this.setTimerInterval(5000);
            this.timer.Tick += new EventHandler(Timer_Tick);
        }

        /// <summary>
        ///     Sets the interval for the tooltip timer, in milliseconds.
        /// </summary>
        /// <param name="milSec">The number of milliseconds.</param>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void setTimerInterval(int milSec)
        {
            this.timer.Interval = milSec;
        }

        /// <summary>
        ///     Timer event fired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eArgs"></param>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            if (sender == this.timer)
            {
                this.hideLastTooltip();
                this.timer.Stop();
            }
        }

        /// <summary>
        ///     Adds a control to the list.
        ///     If the control is already added, then the tooltip text will be updated.
        /// </summary>
        /// <param name="control">The Control.</param>
        /// <param name="tooltipText">The tooltip text to show.</param>
        public void addControl(Control control, String tooltipText)
        {
            if (this.controls.Contains(control))
            {
                this.changeStringForControl(control, tooltipText);
            }
            else
            {
                this.controls.Add(control);
                this.controlTooltips.Add(tooltipText);
            }
        }

        /// <summary>
        ///     Changes the tooltip text for a control.
        /// </summary>
        /// <param name="control">The Control.</param>
        /// <param name="tooltipText">The new tooltip text to show.</param>
        public void changeStringForControl(Control control, String tooltipText)
        {
            for (int i = 0; i < this.controls.Count; i++)
            {
                if (this.controls.ElementAt(i) == control)
                {
                    this.controlTooltips.RemoveAt(i);
                    this.controlTooltips.Insert(i, tooltipText);
                }
            }
        }

        /// <summary>
        ///     Returns the tooltip text of a registered control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>The tooltip text of that control.</returns>
        public String getToolTipTextFromControl(Control control)
        {
            String toolTipText = String.Empty;
            for (int i = 0; i < this.controls.Count; i++)
            {
                if (this.controls.ElementAt(i) == control)
                {
                    toolTipText = this.controlTooltips.ElementAt(i);
                }
            }
            return toolTipText;
        }

        /// <summary>
        ///     Shows the tooltip.
        /// </summary>
        /// <param name="control">The control where the mouse is pointing at.</param>
        /// <remarks>
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>    
        public void updateTooltips(Control control)
        {
            if (control != null) //Catch nullpointer
            {
                for (int i = 0; i < this.controls.Count; i++)
                {
                    if (control == this.controls.ElementAt(i) && !this.controls.ElementAt(i).Enabled && !this.isTooltipShown && this.showTooltips)
                    {
                        this.oldTooltipString = this.oldTooltip.GetToolTip(control);
                        this.oldTooltip.SetToolTip(control, "");
                        this.lastControl = i;
                        tooltip.Show(DomainController.Instance().getLanguageString(this.controlTooltips.ElementAt(i)), this.controls.ElementAt(i), control.Width/2, control.Height/2);
                        this.isTooltipShown = true;
                        this.timer.Start();
                    }
                }
            }
            else 
            {
                this.hideLastTooltip();
            }
        }

        /// <summary>
        ///     Hides the last tooltip.
        /// </summary>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>    
        private void hideLastTooltip()
        {
            if (this.lastControl > -1)
            {
                tooltip.Hide(this.controls.ElementAt(this.lastControl));
                this.oldTooltip.SetToolTip(this.controls.ElementAt(this.lastControl), this.oldTooltipString);
            }
            this.isTooltipShown = false;
        }
    }
}
