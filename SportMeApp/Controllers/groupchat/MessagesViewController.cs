﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportMeApp.Models;

namespace SportMeApp.Controllers.groupchat
{
    public class MessagesViewController : Controller
    {
        private readonly SportMeAppContext _context;

        public MessagesViewController(SportMeAppContext context)
        {
            _context = context;
        }

        // GET: MessagesView
        public async Task<IActionResult> Index()
        {
            var sportMeAppContext = _context.Message.Include(m => m.Group).Include(m => m.User);
            return View(await sportMeAppContext.ToListAsync());
        }

        // GET: MessagesView/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Message == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Group)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: MessagesView/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Set<Group>(), "GroupId", "GroupId");
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "UserId", "UserId");
            return View();
        }

        // POST: MessagesView/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,Text,Timestamp,UserId,GroupId")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Set<Group>(), "GroupId", "GroupId", message.GroupId);
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "UserId", "UserId", message.UserId);
            return View(message);
        }

        // GET: MessagesView/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Message == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Set<Group>(), "GroupId", "GroupId", message.GroupId);
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "UserId", "UserId", message.UserId);
            return View(message);
        }

        // POST: MessagesView/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,Text,Timestamp,UserId,GroupId")] Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Set<Group>(), "GroupId", "GroupId", message.GroupId);
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "UserId", "UserId", message.UserId);
            return View(message);
        }

        // GET: MessagesView/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Message == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Group)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: MessagesView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Message == null)
            {
                return Problem("Entity set 'SportMeAppContext.Message'  is null.");
            }
            var message = await _context.Message.FindAsync(id);
            if (message != null)
            {
                _context.Message.Remove(message);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
          return (_context.Message?.Any(e => e.MessageId == id)).GetValueOrDefault();
        }
    }
}
