﻿using System;
using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Contacts;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.User.Controllers
{
    public class TicketController : UserBaseController
    {
        #region Constructor

        private readonly IContactService _contactService;
        public TicketController(IContactService contactService)
        {
            _contactService = contactService;
        }

        #endregion

        #region List

        [HttpGet("tickets")]
        public async Task<IActionResult> Index(FilterTicketDto filter)
        {
            filter.UserId = User.GetUserId();
            filter.FilterTicketState = FilterTicketState.NotDeleted;
            filter.OrderBy = FilterTicketOrder.CreateDate_DES;

            return View(await _contactService.FilterTickets(filter));
        }

        #endregion

        #region Add Ticket

        [HttpGet("add-ticket")]
        public async Task<IActionResult> AddTicket()
        {
            return View();
        }

        [HttpPost("add-ticket"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicket(AddTicketDto ticket)
        {
            if (ModelState.IsValid)
            {
                var result = await _contactService.AddUserTicket(ticket, User.GetUserId());
                switch (result)
                {
                    case AddTicketResult.Error:
                        TempData[ErrorMessage] = "عملیات با شکست مواجه شد";
                        break;
                    case AddTicketResult.Success:
                        TempData[SuccessMessage] = "تیکت شما با موفقیت ثبت شد";
                        TempData[InfoMessage] = "پاسخ شما به زودی ارسال خواهد شد";
                        return RedirectToAction("Index");
                }
            }

            return View();
        }

        #endregion

        #region Show ticket detail

        [HttpGet("tickets/{ticketId}")]
        public async Task<IActionResult> TicketDetail(long ticketId)
        {
            var ticket = await _contactService.GetTicketForShow(ticketId, User.GetUserId());
            if (ticket == null) return NotFound();

            return View(ticket);
        }

        #endregion

        #region Answer ticket

        [HttpPost("answer-ticket"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AnswerTicket(AnswerTicketDto answer)
        {
            if (string.IsNullOrEmpty(answer.Text))
            {
                TempData[ErrorMessage] = "لطفا متن پیام خود را وارد نمایید";
            }
            if (ModelState.IsValid)
            {
                var result = await _contactService.AnswerTicket(answer, User.GetUserId());
                switch (result)
                {
                    case AnswerTicketResult.NotForUser:
                        TempData[ErrorMessage] = "عدم دسترسی";
                        TempData[InfoMessage] = "در صورت تکرار، دسترسی شما از سیستم قطع خواهد شد";
                        return RedirectToAction("Index");
                    case AnswerTicketResult.NotFound:
                        TempData[WarningMessage] = "اطلاعات مورد نظر یافت نشد";
                        return RedirectToAction("Index");
                    case AnswerTicketResult.Success:
                        TempData[SuccessMessage] = "اطلاعات مورد نظر با موفقیت ثبت شد";
                        break;
                }
            }

            return RedirectToAction("TicketDetail", "Ticket",
                new { area="User", ticketId = answer.Id });
        }

        #endregion
    }
}