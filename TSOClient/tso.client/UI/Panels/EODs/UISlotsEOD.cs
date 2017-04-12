﻿using FSO.Client.UI.Controls;
using FSO.Client.UI.Framework;
using FSO.Client.UI.Framework.Parser;
using FSO.SimAntics.NetPlay.EODs;
using FSO.SimAntics.NetPlay.EODs.Handlers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSO.Client.UI.Panels.EODs
{
    public class UISlotsEOD : UIEOD
    {
        public UIScript Script;
        
        private byte MachineOdds { get; set; }
        private int EachBet;
        private int CurrentBet;
        private int DisplayedBet;
        private short MachineBalance;
        private int OnOffState;
        private int WheelSpinTickCounter = 0;
        private short MachineMinimumBalance;
        private short MachineMaximumBalance;
        private WheelStopsList WheelListOne;
        private WheelStopsList WheelListTwo;
        private WheelStopsList WheelListThree;
        private UILabel ActivePayoutTable;
        private Texture2D ActiveWheelTexture;

        // Owner UI Images
        public UIImage ButtonSeat { get; set; }
        public UIImage TextBack { get; set; }

        // Owner UI Buttons
        public UISlider OddsSlider { get; set; }
        public UIButton OnOffButton { get; set; }
        public UIButton CashOutButton { get; set; }

        // Owner UI Textfields
        public UITextEdit CurrentOdds { get; set; }
        public UITextEdit CashText { get; set; }
        public UILabel Odds { get; set; } // starts at opaque=0
        public UILabel House { get; set; } // starts at opaque=0
        public UILabel Player { get; set; } // starts at opaque=0
        public UILabel OnOff { get; set; } // starts at opaque=0

        // Player UI Images
        public UISlotsImage Wheel1;
        public UISlotsImage Wheel2;
        public UISlotsImage Wheel3;
        public UIImage WinningLine { get; set; }

        public UIImage LightsFrame1 { get; set; }
        public UIImage LightsFrame2 { get; set; }
        public UIImage BetIndents { get; set; }
        public UIImage Wheelsback { get; set; }
        public UISlotsImage Chips { get; set; }

        public UISlotsImage PayoutTableColumn1Row1 { get; set; }
        public UISlotsImage PayoutTableColumn1Row2 { get; set; }
        public UISlotsImage PayoutTableColumn1Row3 { get; set; }
        public UISlotsImage PayoutTableColumn1Row4 { get; set; }
        public UISlotsImage PayoutTableColumn2Row1 { get; set; }
        public UISlotsImage PayoutTableColumn2Row2 { get; set; }
        public UISlotsImage PayoutTableColumn2Row3 { get; set; }
        public UISlotsImage PayoutTableColumn2Row4 { get; set; }

        // Player UI Buttons
        public UIButton ArmButton { get; set; }
        public UIButton SpinButton { get; set; }
        public UIButton SpinnerIncreaseBet { get; set; }
        public UIButton SpinnerDecreaseBet { get; set; }

        // Player UI Textfields
        public UILabel PayoutTable1 { get; set; }
        public UILabel PayoutTable2 { get; set; }
        public UILabel PayoutTable3 { get; set; }
        public UITextEdit BetText { get; set; }
        public UITextEdit PayoutText1 { get; set; }
        public UITextEdit PayoutText2 { get; set; }
        public UITextEdit PayoutText3 { get; set; }
        public UITextEdit PayoutText4 { get; set; }
        public UITextEdit PayoutText5 { get; set; }
        public UITextEdit PayoutText6 { get; set; }
        public UITextEdit PayoutText7 { get; set; }
        public UITextEdit PayoutText8 { get; set; }
        public UILabel Loading { get; set; } // announces wins and losses

        // Textures
        public Texture2D Wheel1Image { get; set; }
        public Texture2D Wheel2Image { get; set; }
        public Texture2D Wheel3Image { get; set; }
        public Texture2D MoneyChipsImage { get; set; }
        public Texture2D LightsFrame1Image { get; set; }
        public Texture2D LightsFrame2Image { get; set; }
        public Texture2D Wheel1LegendImage { get; set; }
        public Texture2D Wheel2LegendImage { get; set; }
        public Texture2D Wheel3LegendImage { get; set; }

        // texutre constants including positions of each slot stop in the texture file
        public const int WHEEL_TEXTURE_WIDTH_AND_HEIGHT = 58;
        public const int WHEEL_TEXTURE_HALF_DRAW_HEIGHT = 29;

        public const int WHEEL_FRAME_CONSTANT = 5;

        // timers for animations
        private Timer OfflineMessageTimer;
        private Timer LightsTimer;
        private Timer WheelsSpinTimer;

        public UISlotsEOD(UIEODController controller) : base(controller)
        {
            Script = this.RenderScript("slotseod.uis");
            PlaintextHandlers["slots_new_game"] = NewGameHandler;
            PlaintextHandlers["slots_display_win"] = DisplayWinHandler;
            PlaintextHandlers["slots_display_loss"] = DisplayLossHandler;
            PlaintextHandlers["slots_close_machine"] = CloseMachineHandler;
            PlaintextHandlers["slots_resume_manage"] = ResumeManageHandler;
            PlaintextHandlers["slots_deposit_NSF"] = DepositFailHandler;
            PlaintextHandlers["slots_withdraw_fail"] = InputFailHandler;
            PlaintextHandlers["slots_deposit_fail"] = InputFailHandler;
            BinaryHandlers["slots_owner_init"] = OwnerInitHandler;
            BinaryHandlers["slots_player_init"] = PlayerInitHandler;
            BinaryHandlers["slots_spin"] = SlotsSpinHandler;

            // Add message text
            Loading.Alignment = TextAlignment.Left;
            Loading.Caption = GameFacade.Strings["UIText", "259", "6"];
        }
        public override void OnExpand()
        {
            ActivePayoutTable.Visible = true;
            PayoutText1.Visible = true;
            PayoutText2.Visible = true;
            PayoutText3.Visible = true;
            PayoutText4.Visible = true;
            PayoutText5.Visible = true;
            PayoutText6.Visible = true;
            PayoutText7.Visible = true;
            PayoutText8.Visible = true;
            PayoutTableColumn1Row1.Visible = true;
            PayoutTableColumn1Row2.Visible = true;
            PayoutTableColumn1Row3.Visible = true;
            PayoutTableColumn1Row4.Visible = true;
            PayoutTableColumn2Row1.Visible = true;
            PayoutTableColumn2Row2.Visible = true;
            PayoutTableColumn2Row3.Visible = true;
            PayoutTableColumn2Row4.Visible = true;
            Loading.Y -= 135;
            base.OnExpand();
        }
        public override void OnContract()
        {
            ActivePayoutTable.Visible = false;
            PayoutText1.Visible = false;
            PayoutText2.Visible = false;
            PayoutText3.Visible = false;
            PayoutText4.Visible = false;
            PayoutText5.Visible = false;
            PayoutText6.Visible = false;
            PayoutText7.Visible = false;
            PayoutText8.Visible = false;
            PayoutTableColumn1Row1.Visible = false;
            PayoutTableColumn1Row2.Visible = false;
            PayoutTableColumn1Row3.Visible = false;
            PayoutTableColumn1Row4.Visible = false;
            PayoutTableColumn2Row1.Visible = false;
            PayoutTableColumn2Row2.Visible = false;
            PayoutTableColumn2Row3.Visible = false;
            PayoutTableColumn2Row4.Visible = false;
            Loading.Y += 135;
            base.OnContract();
        }
        public override void OnClose()
        {
            Send("slots_close_UI", "");
            base.OnClose();
        }
        private void PlayerInitHandler(string evt, byte [] args)
        {
            Controller.ShowEODMode(new EODLiveModeOpt
            {
                Buttons = 0,
                Height = EODHeight.TallTall,
                Length = EODLength.Full,
                Tips = EODTextTips.Short,
                Timer = EODTimer.None,
                Expandable = true,
                Expanded = true
            });
            // hide owner UI elements
            OddsSlider.Visible = false;
            OnOffButton.Visible = false;
            CashOutButton.Visible = false;
            Odds.Visible = false;
            House.Visible = false;
            Player.Visible = false;
            OnOff.Visible = false;
            CashText.Visible = false;

            // move loading
            Loading.X -= 128;
            Loading.Y -= 20;

            // create player UI iamges
            Wheelsback = Script.Create<UIImage>("WheelsBack");
            AddAt(0, Wheelsback);
            LightsFrame1 = Script.Create<UIImage>("LightsFrame1");
            AddAt(1,LightsFrame1);
            LightsFrame2 = Script.Create<UIImage>("LightsFrame2");
            AddAt(2,LightsFrame2);
            BetIndents = Script.Create<UIImage>("BetIndents");
            AddAt(3,BetIndents);
            Chips = new UISlotsImage(MoneyChipsImage);
            Chips.X = 110;
            Chips.Y = 285;
            Add(Chips);

            // Customize and place payout table
            PayoutTableColumn1Row1 = new UISlotsImage(Wheel1LegendImage).TripleTextureDraw(0, 0, 15, 21, 0, 0, 15, 21, 0, 0, 15, 21, true, false);
            PayoutTableColumn1Row1.X = 125;
            PayoutTableColumn1Row1.Y = 120;
            Add(PayoutTableColumn1Row1);
            PayoutTableColumn1Row2 = new UISlotsImage(Wheel1LegendImage).TripleTextureDraw(15, 0, 15, 21, 15, 0, 15, 21, 15, 0, 15, 21, true, false);
            PayoutTableColumn1Row2.X = 125;
            PayoutTableColumn1Row2.Y = 120;
            PayoutTableColumn1Row2.Y += 21;
            Add(PayoutTableColumn1Row2);
            PayoutTableColumn1Row3 = new UISlotsImage(Wheel1LegendImage).TripleTextureDraw(30, 0, 15, 21, 30, 0, 15, 21, 30, 0, 15, 21, true, false);
            PayoutTableColumn1Row3.X = 125;
            PayoutTableColumn1Row3.Y = 120;
            PayoutTableColumn1Row3.Y += 42;
            Add(PayoutTableColumn1Row3);
            PayoutTableColumn1Row4 = new UISlotsImage(Wheel1LegendImage).TripleTextureDraw(45, 0, 15, 21, 45, 0, 15, 21, 45, 0, 15, 21, true, false);
            PayoutTableColumn1Row4.X = 125;
            PayoutTableColumn1Row4.Y = 120;
            PayoutTableColumn1Row4.Y += 63;
            Add(PayoutTableColumn1Row4);
            PayoutTableColumn2Row1 = new UISlotsImage(Wheel1LegendImage).TripleTextureDraw(60, 0, 15, 21, 60, 0, 15, 21, 60, 0, 15, 21, true, false);
            PayoutTableColumn2Row1.X = 275;
            PayoutTableColumn2Row1.Y = 120;
            Add(PayoutTableColumn2Row1);
            PayoutTableColumn2Row2 = new UISlotsImage(Wheel1LegendImage).TripleTextureDraw(45, 0, 15, 21, 60, 0, 15, 21, 75, 0, 15, 21, true, false);
            PayoutTableColumn2Row2.X = 275;
            PayoutTableColumn2Row2.Y = 120;
            PayoutTableColumn2Row2.Y += 21;
            Add(PayoutTableColumn2Row2);
            PayoutTableColumn2Row3 = new UISlotsImage(Wheel1LegendImage).DoubleTextureDraw(75, 0, 15, 21, 75, 0, 15, 21, true, false);
            PayoutTableColumn2Row3.X = 275;
            PayoutTableColumn2Row3.Y = 120;
            PayoutTableColumn2Row3.Y += 42;
            Add(PayoutTableColumn2Row3);
            PayoutTableColumn2Row4 = new UISlotsImage(Wheel1LegendImage);
            PayoutTableColumn2Row4.X = 275;
            PayoutTableColumn2Row4.Y = 120;
            PayoutTableColumn2Row4.SetBounds(75, 0, 15, 21);
            PayoutTableColumn2Row4.Y += 63;
            Add(PayoutTableColumn2Row4);

            // initialize payout textfields, which are currently ubiquitous across all slot machines
            PayoutText1.Y = PayoutTableColumn1Row1.Y - 1;
            PayoutText1.X = PayoutTableColumn1Row1.X + 55;
            PayoutText1.CurrentText = GameFacade.Strings["UIText","259", "36"];
            PayoutText1.CurrentText = PayoutText1.CurrentText.Replace("%i", "" + VMEODSlotsPlugin.SIX_SIX_SIX_PAYOUT_MULTIPLIER);
            PayoutText1.Mode = UITextEditMode.ReadOnly;
            Add(PayoutText1);

            PayoutText2.Y = PayoutTableColumn1Row2.Y - 1;
            PayoutText2.X = PayoutTableColumn1Row2.X + 55;
            PayoutText2.CurrentText = GameFacade.Strings["UIText", "259", "36"];
            PayoutText2.CurrentText = PayoutText2.CurrentText.Replace("%i", "" + VMEODSlotsPlugin.FIVE_FIVE_FIVE_PAYOUT_MULTIPLIER);
            PayoutText2.Mode = UITextEditMode.ReadOnly;
            Add(PayoutText2);

            PayoutText3.Y = PayoutTableColumn1Row3.Y - 1;
            PayoutText3.X = PayoutTableColumn1Row3.X + 55;
            PayoutText3.CurrentText = GameFacade.Strings["UIText", "259", "36"];
            PayoutText3.CurrentText = PayoutText3.CurrentText.Replace("%i", "" + VMEODSlotsPlugin.FOUR_FOUR_FOUR_PAYOUT_MULTIPLIER);
            PayoutText3.Mode = UITextEditMode.ReadOnly;
            Add(PayoutText3);

            PayoutText4.Y = PayoutTableColumn1Row4.Y - 1;
            PayoutText4.X = PayoutTableColumn1Row4.X + 55;
            PayoutText4.CurrentText = GameFacade.Strings["UIText", "259", "36"];
            PayoutText4.CurrentText = PayoutText4.CurrentText.Replace("%i", "" + VMEODSlotsPlugin.THREE_THREE_THREE_PAYOUT_MULTIPLIER);
            PayoutText4.Mode = UITextEditMode.ReadOnly;
            Add(PayoutText4);

            PayoutText5.Y = PayoutTableColumn2Row1.Y - 1;
            PayoutText5.X = PayoutTableColumn2Row1.X + 55;
            PayoutText5.CurrentText = GameFacade.Strings["UIText", "259", "36"];
            PayoutText5.CurrentText = PayoutText5.CurrentText.Replace("%i", "" + VMEODSlotsPlugin.TWO_TWO_TWO_PAYOUT_MULTIPLIER);
            PayoutText5.Mode = UITextEditMode.ReadOnly;
            Add(PayoutText5);

            PayoutText6.Y = PayoutTableColumn2Row2.Y - 1;
            PayoutText6.X = PayoutTableColumn2Row2.X + 55;
            PayoutText6.CurrentText = GameFacade.Strings["UIText", "259", "36"];
            PayoutText6.CurrentText = PayoutText6.CurrentText.Replace("%i", "" + VMEODSlotsPlugin.THREE_TWO_ONE_PAYOUT_MULTIPLIER);
            PayoutText6.Mode = UITextEditMode.ReadOnly;
            Add(PayoutText6);

            PayoutText7.Y = PayoutTableColumn2Row3.Y - 1;
            PayoutText7.X = PayoutTableColumn2Row3.X + 55;
            PayoutText7.CurrentText = GameFacade.Strings["UIText", "259", "36"];
            PayoutText7.CurrentText = PayoutText7.CurrentText.Replace("%i", "" + VMEODSlotsPlugin.ONE_ONE_ANY_PAYOUT_MULTIPLIER);
            PayoutText7.Mode = UITextEditMode.ReadOnly;
            Add(PayoutText7);

            PayoutText8.Y = PayoutTableColumn2Row4.Y - 1;
            PayoutText8.X = PayoutTableColumn2Row4.X + 55;
            PayoutText8.CurrentText = GameFacade.Strings["UIText", "259", "36"];
            PayoutText8.CurrentText = PayoutText8.CurrentText.Replace("%i", "" + VMEODSlotsPlugin.ONE_ANY_ANY_PAYOUT_MULTIPLIER);
            PayoutText8.Mode = UITextEditMode.ReadOnly;
            Add(PayoutText8);

            // create the wheel lists for the spinning
            WheelListOne = new WheelStopsList();
            WheelListTwo = new WheelStopsList();
            WheelListThree = new WheelStopsList();

            // the wheel textures are customised at a later time, but draw the initial pre-gameplay stops (sixth sixth sixth)
            Wheel1 = new UISlotsImage(Wheel1LegendImage).DoubleTextureDraw(0, WheelListOne.Current.MyStartingY, WHEEL_TEXTURE_WIDTH_AND_HEIGHT,
                WHEEL_TEXTURE_HALF_DRAW_HEIGHT, 0, WheelListOne.Current.MyStartingY + WHEEL_TEXTURE_HALF_DRAW_HEIGHT,
                WHEEL_TEXTURE_WIDTH_AND_HEIGHT, WHEEL_TEXTURE_HALF_DRAW_HEIGHT, false, true);
            Wheel1.X = 167;
            Wheel1.Y = 265;
            Wheel2 = new UISlotsImage(Wheel1LegendImage).DoubleTextureDraw(0, WheelListTwo.Current.MyStartingY, WHEEL_TEXTURE_WIDTH_AND_HEIGHT,
                WHEEL_TEXTURE_HALF_DRAW_HEIGHT, 0, WheelListTwo.Current.MyStartingY + WHEEL_TEXTURE_HALF_DRAW_HEIGHT,
                WHEEL_TEXTURE_WIDTH_AND_HEIGHT, WHEEL_TEXTURE_HALF_DRAW_HEIGHT, false, true);
            Wheel2.X = 236;
            Wheel2.Y = 265;
            Wheel3 = new UISlotsImage(Wheel1LegendImage).DoubleTextureDraw(0, WheelListThree.Current.MyStartingY, WHEEL_TEXTURE_WIDTH_AND_HEIGHT,
                WHEEL_TEXTURE_HALF_DRAW_HEIGHT, 0, WheelListThree.Current.MyStartingY + WHEEL_TEXTURE_HALF_DRAW_HEIGHT,
                WHEEL_TEXTURE_WIDTH_AND_HEIGHT, WHEEL_TEXTURE_HALF_DRAW_HEIGHT, false, true);
            Wheel3.X = 305;
            Wheel3.Y = 265;

            Add(Wheel1);
            Add(Wheel2);
            Add(Wheel3);
            WinningLine = Script.Create<UIImage>("WinningLine");
            Add(WinningLine);

            if ((args != null) && (args.Length > 1))
                MachineTypeInit(args[1]);
            else
                MachineTypeInit(0);

            // create a timer to animate the lights, milliseconds
            LightsTimer = new System.Timers.Timer(666 + (2 / 3));
            LightsTimer.Elapsed += new ElapsedEventHandler(LightsHandler);

            // create a timer to change offline messages
            OfflineMessageTimer = new System.Timers.Timer(3000);
            OfflineMessageTimer.Elapsed += new ElapsedEventHandler(OfflineMessageHandler);

            // create a timer to handle the spinning of the wheels
            WheelsSpinTimer = new System.Timers.Timer(25);
            WheelsSpinTimer.Elapsed += new ElapsedEventHandler(AnimateWheelsHandler);
        }

        private void OwnerInitHandler(string evt, byte[] args)
        {
            Controller.ShowEODMode(new EODLiveModeOpt
            {
                Buttons = 0,
                Height = EODHeight.Normal,
                Length = EODLength.Full,
                Tips = EODTextTips.Short,
                Timer = EODTimer.None,
                Expandable = false
            });

            // create owner UIImages
            ButtonSeat = Script.Create<UIImage>("ButtonSeat");
            TextBack = Script.Create<UIImage>("TextBack");
            AddAt(0, ButtonSeat);
            AddAt(0, TextBack);

            // hide player UI elements
            ArmButton.Visible = false;
            SpinButton.Visible = false;
            SpinnerIncreaseBet.Visible = false;
            SpinnerDecreaseBet.Visible = false;
            PayoutTable1.Visible = false;
            PayoutTable2.Visible = false;
            PayoutTable3.Visible = false;
            BetText.Visible = false;
            PayoutText1.Visible = false;
            PayoutText2.Visible = false;
            PayoutText3.Visible = false;
            PayoutText4.Visible = false;
            PayoutText5.Visible = false;
            PayoutText6.Visible = false;
            PayoutText7.Visible = false;
            PayoutText8.Visible = false;

            // Move the messages, tweaks
            Loading.X = 72;
            Loading.Y = 6;
            Loading.Caption = GameFacade.Strings["UIText", "259", "24"];
            OnOff.Y = Odds.Y;
            OnOff.X += 15;
            OnOffButton.X += 15;
            Odds.X += 18;
            Player.X += 10;
            House.X += 10;
            OddsSlider.X += 25;

            // Set the Odds slider
            if ((args == null) || (args.Length < 1) || (args[0] < 80))
                MachineOdds = 80;
            else if (args[0] > 110)
                MachineOdds = 110;
            else
                MachineOdds = args[0];
            OddsSlider.Tooltip = MachineOdds + "%";
            OddsSlider.MinValue = 80;
            OddsSlider.MaxValue = 110;
            OddsSlider.Value = MachineOdds;
            CurrentOdds = new UITextEdit();
            CurrentOdds.Size = new Microsoft.Xna.Framework.Vector2(45, 20);
            CurrentOdds.Y = House.Y;
            CurrentOdds.X = Odds.X + 46;
            CurrentOdds.Mode = UITextEditMode.ReadOnly;
            CurrentOdds.CurrentText = MachineOdds + "%";
            Add(CurrentOdds);
            Player.Y = House.Y = CashText.Y;
            OddsSlider.Y += 18;

            // initiate OnOffButton
            if ((args != null) && (args.Length > 3))
            {
                OnOffButton.ForceState = OnOffState = args[3];
                if (args[3] == 1)
                {
                    OnOffButton.Tooltip = GameFacade.Strings["UIText", "259", "14"];
                }
                else
                    OnOffButton.Tooltip = GameFacade.Strings["UIText", "259", "13"];
            }
            else
            {
                OnOffButton.ForceState = OnOffState = 0;
                OnOffButton.Tooltip = GameFacade.Strings["UIText", "259", "13"];
            }

            // calculate the money in the machine from the two shorts and populate textField
            if ((args != null) && (args.Length > 2))
                MachineBalance = Convert.ToInt16((255 * args[2]) + args[1]);
            else
                MachineBalance = 0;
            CashText.Alignment = TextAlignment.Center;
            CashText.Mode = UITextEditMode.ReadOnly;
            CashText.CurrentText = "$" + MachineBalance;

            // add click listeners
            OnOffButton.OnButtonClick += OnOffHandler;
            OddsSlider.OnChange += OddsChangeHandler;
            CashOutButton.OnButtonClick += CashoutButtonHandler;

            // get the minimum and maximum balances based on the machine type
            if ((args != null) && (args.Length > 3))
            {
                switch (args[4])
                {
                    case 0:
                        MachineMinimumBalance = (short)VMEODSlotMachineMinimumBalances.Viva_PGT;
                        MachineMaximumBalance = (short)VMEODSlotMachineMaximumBalances.Viva_PGT;
                        break;
                    case 1:
                        MachineMinimumBalance = (short)VMEODSlotMachineMinimumBalances.Gypsy_Queen;
                        MachineMaximumBalance = (short)VMEODSlotMachineMaximumBalances.Gypsy_Queen;
                        break;
                    default:
                        MachineMinimumBalance = (short)VMEODSlotMachineMinimumBalances.Jack_of_Hearts;
                        MachineMaximumBalance = (short)VMEODSlotMachineMaximumBalances.Jack_of_Hearts;
                        break;
                }
            }
        }
        private void CashoutButtonHandler(UIElement targetButton)
        {
            CashOutButton.OnButtonClick -= CashoutButtonHandler;

            // show an alert that asks the user if they want to make a desposit or a withdrawal
            UIAlert alert = null;
            alert = UIScreen.GlobalShowAlert(new UIAlertOptions()
            {
                TextSize = 12,
                Title = "Owner Transactions",
                Message = "What would you like to do?",
                Alignment = TextAlignment.Center,
                TextEntry = false,
                Buttons = new UIAlertButton[] {
                    new UIAlertButton (UIAlertButtonType.OK, ((btn1) =>
                {
                    DepositPrompt();
                    UIScreen.RemoveDialog(alert);
                }), "Deposit"),
                    new UIAlertButton (UIAlertButtonType.Cancel, ((btn2) =>
                {
                    WithdrawPrompt();
                    UIScreen.RemoveDialog(alert);
                }), "Withdraw")
                }
            }, true);
        }
        private void DepositPrompt()
        {
            // show an alert that asks the user how much to deposit into the machine
            UIAlert alert = null;
            alert = UIScreen.GlobalShowAlert(new UIAlertOptions()
            {
                TextSize = 12,
                Title = "Deposit Simoleons",
                Message = "This machine is currently stocked with: $" + MachineBalance + System.Environment.NewLine +
                System.Environment.NewLine + "For players to use this machine you must maintain a minimum balance of: $" + MachineMinimumBalance +
                System.Environment.NewLine + System.Environment.NewLine + "How much would you like to deposit?" +
                System.Environment.NewLine + System.Environment.NewLine + "(This machine cannot hold more than: $" + MachineMaximumBalance + ")",
                Alignment = TextAlignment.Left,
                TextEntry = true,
                Buttons = UIAlertButton.Ok((btn) =>
                {
                    InputHandler("d", alert.ResponseText.Trim());
                    UIScreen.RemoveDialog(alert);
                }),

            }, true);
        }
        private void WithdrawPrompt()
        {
            // show an alert that asks the user how much to withdraw from the machine
            UIAlert alert = null;
            alert = UIScreen.GlobalShowAlert(new UIAlertOptions()
            {
                TextSize = 12,
                Title = "Withdraw Simoleons",
                Message = "This machine is currently stocked with: $" + MachineBalance + System.Environment.NewLine +
                System.Environment.NewLine + "For players to use this machine you must maintain a minimum balance of: $" + MachineMinimumBalance +
                System.Environment.NewLine + System.Environment.NewLine + "How much would you like to withdraw?",
                Alignment = TextAlignment.Left,
                TextEntry = true,
                Buttons = UIAlertButton.Ok((btn) =>
                {
                    InputHandler("w", alert.ResponseText.Trim());
                    UIScreen.RemoveDialog(alert);
                }),

            }, true);
        }
        private void InputHandler(string type, string userInput)
        {
            short amount;
            userInput.Replace("-", ""); // in case any jokesters try to input a negative number
            string eventName = "";
            string eventMessage = "";
            // try to parse the user's input
            try
            {
                amount = Int16.Parse(userInput);
                // input is valid, now check it against MachineBalance
                if (amount == 0)
                {
                    eventName = null;
                    eventMessage = VMEODSlotsInputErrorTypes.Null.ToString();
                }
                else if (type.Equals("w")) { // withdrawing
                    if (amount > MachineBalance)
                    {
                        eventName = null;
                        eventMessage = VMEODSlotsInputErrorTypes.Overflow.ToString();
                    }
                    else
                    {
                        eventName = "slots_withdraw";
                        eventMessage = "" + amount;
                    }
                }
                else // depositing
                {
                    if ((amount + MachineBalance) > MachineMaximumBalance)
                    {
                        eventName = null;
                        eventMessage = VMEODSlotsInputErrorTypes.Overflow.ToString();
                    }
                    else
                    {
                        eventName = "slots_deposit";
                        eventMessage = "" + amount;
                    }
                }
            }
            catch (ArgumentNullException nullException)
            {
                eventName = null;
                eventMessage = VMEODSlotsInputErrorTypes.Null.ToString();
            }
            catch (FormatException formatException)
            {
                eventName = null;
                if (userInput.Length == 0)
                    eventMessage = VMEODSlotsInputErrorTypes.Null.ToString();
                else
                    eventMessage = VMEODSlotsInputErrorTypes.Invalid.ToString();
            }
            catch (OverflowException overFlowException)
            {
                eventName = null;
                eventMessage = VMEODSlotsInputErrorTypes.Overflow.ToString();
            }
            finally
            {
                if (eventName != null)
                    Send(eventName, eventMessage);
                else
                {
                    if (type.Equals("w"))
                        InputFailHandler("slots_withdraw_fail", eventMessage);
                    else
                        InputFailHandler("slots_deposit_fail", eventMessage);
                }
            }
        }
        private void InputFailHandler(string transactionType, string failureReason)
        {
            string message = "";
            if (failureReason.Equals(VMEODSlotsInputErrorTypes.Null.ToString()))
            {
                ResumeManageHandler("slots_resume_manage", "" + MachineBalance);
                return;
            }
            else if (failureReason.Equals(VMEODSlotsInputErrorTypes.Invalid.ToString()))
                message = "That is not a valid number!";
            else if (failureReason.Equals(VMEODSlotsInputErrorTypes.Overflow.ToString()))
            {
                if (transactionType.Equals("slots_withdraw_fail"))
                    message = "You cannot withdraw more than the balance of the machine!";
                else
                    message = "You cannot deposit that many simoleons because the machine can only hold: $" + MachineMaximumBalance;
            }
            else
                message = "An unknown error occured.";
            UIAlert alert = null;
            alert = UIScreen.GlobalShowAlert(new UIAlertOptions()
            {
                TextSize = 12,
                Title = "Transaction Error",
                Message = message,
                Alignment = TextAlignment.Center,
                TextEntry = false,
                Buttons = UIAlertButton.Ok((btn) =>
                {
                    ResumeManageHandler("slots_resume_manage", "" + MachineBalance);
                    UIScreen.RemoveDialog(alert);
                }),

            }, true);
        }
        private void DepositFailHandler(string evt, string amountString)
        {
            // show an alert that informs the user that they don't have the money to make the deposit
            UIAlert alert = null;
            alert = UIScreen.GlobalShowAlert(new UIAlertOptions()
            {
                TextSize = 12,
                Title = "Transaction Error",
                Message = "You don't have enough simoleons to deposit: $" + amountString,
                Alignment = TextAlignment.Center,
                TextEntry = false,
                Buttons = UIAlertButton.Ok((btn) =>
                {
                    ResumeManageHandler("slots_resume_manage", "" + MachineBalance);
                    UIScreen.RemoveDialog(alert);
                }),

            }, true);
        }
        private void ResumeManageHandler(string evt, string balance)
        {
            short newBalance;
            var result = Int16.TryParse(balance, out newBalance);
            if (result)
                MachineBalance = newBalance;
            CashText.CurrentText = "$" + MachineBalance;
            CashOutButton.OnButtonClick += CashoutButtonHandler;
        }
        private void OddsChangeHandler(UIElement targetSlider)
        {
            OddsSlider.OnChange -= OddsChangeHandler;
            MachineOdds = Convert.ToByte(OddsSlider.Value);
            Send("slots_new_odds", new Byte[] { MachineOdds });
            OddsSlider.Tooltip = MachineOdds + "%";
            CurrentOdds.CurrentText = MachineOdds + "%";
            OddsSlider.OnChange += OddsChangeHandler;
        }
        private void OnOffHandler(UIElement targetButton)
        {
            OnOffButton.Disabled = true;
            if (OnOffState == 0)
            {
                OnOffButton.ForceState = OnOffState = 1;
                OnOffButton.Tooltip = GameFacade.Strings["UIText", "259", "14"];
            }
            else
            {
                OnOffButton.ForceState = OnOffState = 0;
                OnOffButton.Tooltip = GameFacade.Strings["UIText", "259", "13"];
            }
            Send("slots_toggle_onOff", "" + OnOffState);
            OnOffButton.Disabled = false;
        }
        private void NewGameHandler(String evt, String message)
        {
            WheelSpinTickCounter = 0;
            LightsTimer.Stop();
            LightsTimer.Interval = 666 + (2 / 3);
            LightsTimer.Start();
            AddPlayerListeners();
            WheelListOne.Reset();
            WheelListTwo.Reset();
            WheelListThree.Reset();

            if (Loading.Caption == GameFacade.Strings["UIText", "259", "6"])
                Loading.Caption = GameFacade.Strings["UIText", "259", "18"];
        }
        private void BetIncreaseButtonPressedHandler(UIElement targetButton)
        {
            RemovePlayerListeners();
            if (CurrentBet == 5)
            {
                // do nothing, cannot bet more than 5 coins
            }
            else
            {
                CurrentBet++;
                DisplayedBet = CurrentBet * EachBet;
                UpdateBetText();
            }
            AddPlayerListeners();
        }
        private void BetDecreaseButtonPressedHandler(UIElement targetButton)
        {
            RemovePlayerListeners();
            if (CurrentBet == 1)
            {
                // do nothing, cannot bet less than 1 coin
            }
            else
            {
                CurrentBet--;
                DisplayedBet = CurrentBet * EachBet;
                UpdateBetText();
            }
            AddPlayerListeners();
        }
        private void SpinButtonPressedHandler(UIElement targetButton)
        {
            RemovePlayerListeners();
            Send("slots_execute_bet", "" + DisplayedBet);
        }
        private void SlotsSpinHandler(string evt, Byte [] TargetStops)
        {
            LightsTimer.Interval = 500;

            // update text field to mention bet amount
            Loading.Caption = GameFacade.Strings["UIText", "259", "21"];
            Loading.Caption = Loading.Caption.Replace("%i", "" + DisplayedBet);

            // get the three target stops, recast them as type EMOEODSlotsStops
            /*WheelListOne.TargetStop = (EMOEDSlotsStops)Enum.Parse(typeof(EMOEDSlotsStops), (Enum.GetName(typeof(EMOEDSlotsStops),
                WinningsandTargetStops[2]))); depricated approach */
            WheelListOne.TargetStop = (VMEODSlotsStops)Enum.ToObject(typeof(VMEODSlotsStops), TargetStops[0]);
            WheelListTwo.TargetStop = (VMEODSlotsStops)Enum.ToObject(typeof(VMEODSlotsStops), TargetStops[1]);
            WheelListThree.TargetStop = (VMEODSlotsStops)Enum.ToObject(typeof(VMEODSlotsStops), TargetStops[2]);
            WheelsSpinTimer.Start();
        }
        private void DisplayLossHandler(string evt, string stringNumber)
        {
            LightsTimer.Interval = 666 + (2 / 3);
            Loading.Caption = GameFacade.Strings["UIText", "259", stringNumber];
        }
        private void DisplayWinHandler(string evt, string stringNumber)
        {
            LightsTimer.Interval = 100;
            var data = stringNumber.Split('%');
            Loading.Caption = GameFacade.Strings["UIText", "259", data[0]];
            Loading.Caption = Loading.Caption.Replace("%i", data[1]);
        }
        private void AnimateWheelsHandler(object source, ElapsedEventArgs args)
        {
            WheelSpinTickCounter++;

            if (WheelSpinTickCounter >= 160)
            {
                if (WheelSpinTickCounter == 160)
                {
                    WheelListOne.Start();
                }
                else if (WheelSpinTickCounter == 162)
                {
                    WheelListTwo.Start();
                }
                else if (WheelSpinTickCounter == 164)
                {
                    WheelListThree.Start();
                }
                else if (WheelSpinTickCounter == 184)
                {
                    WheelListOne.SlowDown();
                }
                else if (WheelSpinTickCounter == 214)
                {
                    WheelListTwo.SlowDown();
                }
                else if (WheelSpinTickCounter == 244)
                {
                    WheelListThree.SlowDown();
                }
                // 40 ticks times 7 seconds = 280
                else if (WheelSpinTickCounter == 280)
                {
                    // tell the plugin that the wheels have stopped spinning
                    WheelsSpinTimer.Stop();
                    Send("slots_wheels_stopped", "");
                }
                else
                {

                }
                WheelListOne.Spin();
                WheelListTwo.Spin();
                WheelListThree.Spin();
                // draw the wheel stops based on updated positions
                DrawWheelStops(WheelListOne.IsSpinFinished, WheelListTwo.IsSpinFinished, WheelListThree.IsSpinFinished);
            }
        }
        private void CloseMachineHandler(string evt, string msg)
        {
            LightsTimer.Stop();
            OfflineMessageTimer.Start();
            BetText.Visible = false;
            Loading.Caption = GameFacade.Strings["UIText", "259", "22"];
        }
        private void LightsHandler(object source, ElapsedEventArgs args)
        {
            if (LightsFrame2 == null) { return; }
            else if (LightsFrame2.Visible == true)
                LightsFrame2.Visible = false;
            else
                LightsFrame2.Visible = true;
        }
        private void OfflineMessageHandler(object source, ElapsedEventArgs args)
        {
            if (Loading.Caption.Equals(GameFacade.Strings["UIText", "259", "22"]))
            {
                Loading.Caption = GameFacade.Strings["UIText", "259", "23"];
            }
            else
            {
                Loading.Caption = GameFacade.Strings["UIText", "259", "22"];
            }
        }
        private void DrawWheelStops(bool wheelOneAlreadyDone, bool wheelTwoAlreadyDone, bool wheelThreeAlreadyDone)
        {
            if (wheelOneAlreadyDone == false)  // do not redraw if wheel hasn't moved
            {
                Remove(Wheel1);
                Wheel1 = new UISlotsImage(ActiveWheelTexture).DoubleTextureDraw(0, WheelListOne.Next.MyStartingY +
                    (WHEEL_TEXTURE_WIDTH_AND_HEIGHT - WheelListOne.OffsetY), WHEEL_TEXTURE_WIDTH_AND_HEIGHT, WheelListOne.OffsetY,
                    0, WheelListOne.Current.MyStartingY, WHEEL_TEXTURE_WIDTH_AND_HEIGHT, (WHEEL_TEXTURE_WIDTH_AND_HEIGHT - WheelListOne.OffsetY),
                    false, true);
                Wheel1.X = 167;
                Wheel1.Y = 265;
                AddBefore(Wheel1, WinningLine);
            }
            if (wheelTwoAlreadyDone == false) // do not redraw if wheel hasn't moved
            {
                Remove(Wheel2);
                Wheel2 = new UISlotsImage(ActiveWheelTexture).DoubleTextureDraw(0, WheelListTwo.Next.MyStartingY +
                (WHEEL_TEXTURE_WIDTH_AND_HEIGHT - WheelListTwo.OffsetY), WHEEL_TEXTURE_WIDTH_AND_HEIGHT, WheelListTwo.OffsetY,
                0, WheelListTwo.Current.MyStartingY, WHEEL_TEXTURE_WIDTH_AND_HEIGHT, (WHEEL_TEXTURE_WIDTH_AND_HEIGHT - WheelListTwo.OffsetY),
                false, true);
                Wheel2.X = 236;
                Wheel2.Y = 265;
                AddBefore(Wheel2, WinningLine);
            }
            if (wheelThreeAlreadyDone == false) // do not redraw if wheel hasn't moved
            {
                Remove(Wheel3);
                Wheel3 = new UISlotsImage(ActiveWheelTexture).DoubleTextureDraw(0, WheelListThree.Next.MyStartingY +
                 (WHEEL_TEXTURE_WIDTH_AND_HEIGHT - WheelListThree.OffsetY), WHEEL_TEXTURE_WIDTH_AND_HEIGHT, WheelListThree.OffsetY,
                 0, WheelListThree.Current.MyStartingY, WHEEL_TEXTURE_WIDTH_AND_HEIGHT, (WHEEL_TEXTURE_WIDTH_AND_HEIGHT - WheelListThree.OffsetY),
                 false, true);
                Wheel3.X = 305;
                Wheel3.Y = 265;
                AddBefore(Wheel3, WinningLine);
            }
        }
        /*
         * @param machineGrade: '0' is $1 slot machine, '1' is $5, '2' is $10
         */
        private void MachineTypeInit(byte machineGrade)
        {
            // customise wheel textures by machine type
            switch (machineGrade)
            {
                case 0:
                    Wheel1.Texture = Wheel1Image;
                    Wheel2.Texture = Wheel1Image;
                    Wheel3.Texture = Wheel1Image;
                    ActiveWheelTexture = Wheel1Image;
                    PayoutTableColumn1Row1.Texture = Wheel1LegendImage;
                    PayoutTableColumn1Row2.Texture = Wheel1LegendImage;
                    PayoutTableColumn1Row3.Texture = Wheel1LegendImage;
                    PayoutTableColumn1Row4.Texture = Wheel1LegendImage;
                    PayoutTableColumn2Row1.Texture = Wheel1LegendImage;
                    PayoutTableColumn2Row2.Texture = Wheel1LegendImage;
                    PayoutTableColumn2Row3.Texture = Wheel1LegendImage;
                    PayoutTableColumn2Row4.Texture = Wheel1LegendImage;
                    PayoutTable1.X -= 72;
                    PayoutTable1.Y -= 5;
                    PayoutTable2.Visible = false;
                    PayoutTable3.Visible = false;
                    ActivePayoutTable = PayoutTable1;
                    EachBet = 1;
                    break;
                case 1:
                    Wheel1.Texture = Wheel2Image;
                    Wheel2.Texture = Wheel2Image;
                    Wheel3.Texture = Wheel2Image;
                    ActiveWheelTexture = Wheel2Image;
                    PayoutTableColumn1Row1.Texture = Wheel2LegendImage;
                    PayoutTableColumn1Row2.Texture = Wheel2LegendImage;
                    PayoutTableColumn1Row3.Texture = Wheel2LegendImage;
                    PayoutTableColumn1Row4.Texture = Wheel2LegendImage;
                    PayoutTableColumn2Row1.Texture = Wheel2LegendImage;
                    PayoutTableColumn2Row2.Texture = Wheel2LegendImage;
                    PayoutTableColumn2Row3.Texture = Wheel2LegendImage;
                    PayoutTableColumn2Row4.Texture = Wheel2LegendImage;
                    PayoutTable2.X -= 30;
                    PayoutTable2.Y -= 5;
                    PayoutTable1.Visible = false;
                    PayoutTable3.Visible = false;
                    ActivePayoutTable = PayoutTable2;
                    EachBet = 5;
                    break;
                case 2:
                    Wheel1.Texture = Wheel3Image;
                    Wheel2.Texture = Wheel3Image;
                    Wheel3.Texture = Wheel3Image;
                    ActiveWheelTexture = Wheel3Image;
                    PayoutTableColumn1Row1.Texture = Wheel3LegendImage;
                    PayoutTableColumn1Row2.Texture = Wheel3LegendImage;
                    PayoutTableColumn1Row3.Texture = Wheel3LegendImage;
                    PayoutTableColumn1Row4.Texture = Wheel3LegendImage;
                    PayoutTableColumn2Row1.Texture = Wheel3LegendImage;
                    PayoutTableColumn2Row2.Texture = Wheel3LegendImage;
                    PayoutTableColumn2Row3.Texture = Wheel3LegendImage;
                    PayoutTableColumn2Row4.Texture = Wheel3LegendImage;
                    PayoutTable3.X -= 32;
                    PayoutTable3.Y -= 5;
                    PayoutTable1.Visible = false;
                    PayoutTable2.Visible = false;
                    ActivePayoutTable = PayoutTable3;
                    EachBet = 10;
                    break;
                default:
                    break;
            }
            // customise the chips texture
            Chips.SetBounds(machineGrade * 27, 0, 27, 20);
            // customise bet text
            BetText.Alignment = TextAlignment.Center;
            BetText.Mode = UITextEditMode.ReadOnly;
            Add(BetText);
            CurrentBet = 1;
            DisplayedBet = CurrentBet * EachBet;
            UpdateBetText();
        }
        private void UpdateBetText()
        {
            BetText.CurrentText = "$" + DisplayedBet;
        }
        private void RemovePlayerListeners()
        {
            try
            {
                ArmButton.OnButtonClick -= SpinButtonPressedHandler;
                SpinButton.OnButtonClick -= SpinButtonPressedHandler;
                SpinnerIncreaseBet.OnButtonClick -= BetIncreaseButtonPressedHandler;
                SpinnerDecreaseBet.OnButtonClick -= BetDecreaseButtonPressedHandler;
            }
            catch (Exception error)
            {
                Console.WriteLine("UISLotsEOD.RemoveAllListeners: There was an error: " + error);
                ArmButton.Disabled = true;
                SpinButton.Disabled = true;
                SpinnerIncreaseBet.Disabled = true;
                SpinnerDecreaseBet.Disabled = true;
            }
        }
        private void AddPlayerListeners() {
            ArmButton.OnButtonClick += SpinButtonPressedHandler;
            SpinButton.OnButtonClick += SpinButtonPressedHandler;
            SpinnerIncreaseBet.OnButtonClick += BetIncreaseButtonPressedHandler;
            SpinnerDecreaseBet.OnButtonClick += BetDecreaseButtonPressedHandler;
            ArmButton.Disabled = false;
            SpinButton.Disabled = false;
            SpinnerIncreaseBet.Disabled = false;
            SpinnerDecreaseBet.Disabled = false;
        }
    }
    class WheelStopsList
    {
        public WheelStopNode Current;
        public WheelStopNode Next;
        public WheelStopNode Previous;
        private int MyOffsetY = 0;
        private int CurrentSpeedCounter = 0;
        private int CurrentSpeed = 0;
        private int TicksToStop = 36;
        private int TargetStopsAway = 0;
        private int DistanceToTarget = -1;
        private Random RandomLeftover = new Random();
        private int LeftoverY = 0;
        public VMEODSlotsStops TargetStop;
        public UISlotsEODSlotWheelStates State;
        private bool HasStarted;
        public bool IsSpinFinished;

        public WheelStopsList()
        {
            // make a node for each possible stop in the correct order
            foreach (VMEODSlotsStops stop in Enum.GetValues(typeof(VMEODSlotsStops)))
            {
                if (Previous != null)
                {
                    Current = new WheelStopNode(stop);
                    Previous.Next = Current;
                }
                else
                {
                    Next = Current = new WheelStopNode(stop);
                }
                Previous = Current;
            }
            // close the loop by settings top-most's (the jackspot/sixth stop) Next to bottom-most, the first one made (blankOne)
            Current.Next = Next; 

            // Previous is not used during gameplay, so setting it to be a BLANK stop will serve a purpose when the wheel stops
            Previous = Next;

            // begin stopped
            State = UISlotsEODSlotWheelStates.Stopped;
        }
        public int OffsetY
        {
            get { return MyOffsetY; }
        }
        public void Spin()
        {
            switch (State)
            {
                case UISlotsEODSlotWheelStates.Stopped:
                    {
                        IsSpinFinished = true;
                        break;
                    }
                case UISlotsEODSlotWheelStates.Spinning:
                    {
                        MyOffsetY += CurrentSpeed;
                        break;
                    }
                case UISlotsEODSlotWheelStates.Starting:
                    {
                        if (CurrentSpeedCounter == 2)
                        {
                            // if at the penultimate speed, skip to ultimate
                            if (CurrentSpeed == 22)
                            {
                                CurrentSpeed = 29;
                                CurrentSpeedCounter = 0;
                                State = UISlotsEODSlotWheelStates.Spinning;
                            }
                            // increase the speed
                            else
                            {
                                CurrentSpeed += UISlotsEOD.WHEEL_FRAME_CONSTANT;
                                CurrentSpeedCounter = 0;
                            }
                        }
                        MyOffsetY += CurrentSpeed;
                        CurrentSpeedCounter++;
                        break;
                    }
                case UISlotsEODSlotWheelStates.Slowing:
                    {
                        // still move at full speak since target is far away
                        if (DistanceToTarget > 116)
                        {
                            CurrentSpeed = 29;
                        }
                        // start to slow down
                        else if (DistanceToTarget == 116)
                        {
                            CurrentSpeed = 22;
                            CurrentSpeedCounter = 0;
                        }
                        // 0 > DistanceToTarget < 116
                        else
                        {
                            // check if this is first or second time at CurrentSpeed
                            if (CurrentSpeedCounter == 2)
                            {
                                // if Current speed is slowest, make it stop
                                if (CurrentSpeed == 7)
                                {
                                    CurrentSpeed = 0;
                                    if (TicksToStop > 5) // stopped early
                                        TicksToStop = 5;

                                    MyOffsetY = LeftoverY = RandomLeftover.Next(0, TicksToStop);
                                    State = UISlotsEODSlotWheelStates.Stopping;
                                }
                                // otherwise, go even slower
                                else
                                {
                                    CurrentSpeed -= UISlotsEOD.WHEEL_FRAME_CONSTANT;
                                    CurrentSpeedCounter = 0;
                                }
                            }
                        }
                        TicksToStop--;
                        CurrentSpeedCounter++;
                        DistanceToTarget -= CurrentSpeed;
                        MyOffsetY += CurrentSpeed;
                        break;
                    }
                case UISlotsEODSlotWheelStates.Stopping:
                    {
                        if ((TicksToStop == 0))
                        {
                            MyOffsetY = LeftoverY = 0;
                            State = UISlotsEODSlotWheelStates.Stopped;
                            break;
                        }
                        else if (TicksToStop % 2 == 1)
                        {
                            MyOffsetY = LeftoverY = (LeftoverY / 2);
                        }
                        TicksToStop--;
                        break;
                    }
            }
            // checking if list needs to update
            if (MyOffsetY >= UISlotsEOD.WHEEL_TEXTURE_WIDTH_AND_HEIGHT)
            {
                MyOffsetY -= UISlotsEOD.WHEEL_TEXTURE_WIDTH_AND_HEIGHT;
                Previous = Current;
                Current = Next;
                Next = Next.Next;
            }
        }
        public void Start()
        {
            CurrentSpeed = UISlotsEOD.WHEEL_FRAME_CONSTANT + 2;
            CurrentSpeedCounter = 0;
            HasStarted = true;
            IsSpinFinished = false;
            State = UISlotsEODSlotWheelStates.Starting;
        }
        public void SlowDown()
        {
            TargetStopsAway = 0;
            var tempNode = new WheelStopNode(Current.MyStop, Current);
            while (!tempNode.Next.MyStop.Equals(TargetStop))
            {
                TargetStopsAway++;
                tempNode = tempNode.Next;
            }
            if (TargetStopsAway < 3) // TargetStop is too close to make animation fun
            {
                TargetStopsAway += 12;
            }
            // calculate distance away
            DistanceToTarget = ((TargetStopsAway * UISlotsEOD.WHEEL_TEXTURE_WIDTH_AND_HEIGHT) - MyOffsetY);
            State = UISlotsEODSlotWheelStates.Slowing;
        }
        public void Reset()
        {
            State = UISlotsEODSlotWheelStates.Stopped;
            CurrentSpeedCounter = 0;
            CurrentSpeed = 0;
            LeftoverY = 0;
            TicksToStop = 36;
            DistanceToTarget = -1;
            HasStarted = false;
        }
    }
    class WheelStopNode
    {
        public WheelStopNode Next { get; set; }
        public VMEODSlotsStops MyStop;
        public int MyStartingY;

        public WheelStopNode(VMEODSlotsStops stop)
        {
            MyStop = stop;
            CalculateStartingY();
        }
        public WheelStopNode(VMEODSlotsStops stop, WheelStopNode next)
        {
            MyStop = stop;
            Next = next;
            CalculateStartingY();
        }
        public void CalculateStartingY()
        {
            if ((byte)MyStop % 2 == 0) // a blank stop
            {
                MyStartingY = 6 * UISlotsEOD.WHEEL_TEXTURE_WIDTH_AND_HEIGHT;
            }
            else
            {
                var stopVal = (byte)MyStop;
                MyStartingY = (5 - ((stopVal - 1) / 2)) * UISlotsEOD.WHEEL_TEXTURE_WIDTH_AND_HEIGHT;
            }
        }
    }
    public enum UISlotsEODSlotWheelStates : byte
    {
        Stopped = 0,
        Starting = 1,
        Spinning = 2,
        Slowing = 3,
        Stopping = 4
    }
}
