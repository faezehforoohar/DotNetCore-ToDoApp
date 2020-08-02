using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using ToDoApp.Services;

namespace ToDoApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;
        public TasksController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public ActionResult<List<Models.Task>> Get()
        {
            List<Models.Task> lst = _taskService.Get();

            return Ok(new { lst, result = true });
        }

        [HttpGet("{id:length(24)}")]
        public ActionResult<Models.Task> Get(string id)
        {
            var task = _taskService.Get(id);

            if (task == null)
                return Ok(new { message = "Invalid Data", result = false });
            return Ok(new { task, result = true });
        }

        [HttpPost]
        public ActionResult<Models.Task> Post(Models.Task task)
        {
            try
            {
                if (task.Id == "0" || string.IsNullOrWhiteSpace(task.Id))
                {
                    task.Id = ObjectId.GenerateNewId().ToString();
                    _taskService.Create(task);
                }
                else
                {
                    var task_prev = _taskService.Get(task.Id);

                    if (task_prev == null)
                        return Ok(new { message = "Invalid Data", result = false });
                    _taskService.Update(task.Id, task);
                }
                return Ok(new { message = "Succeed", result = true });
            }
            catch (Exception ex)
            {
                return Ok(new { message = ex.Message, result = false });
            }
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var task = _taskService.Get(id);

                if (task == null)
                    return Ok(new { message = "Invalid Data", result = false });
                _taskService.Remove(task.Id);
                return Ok(new { message = "Deleting was Successful.", result = true });
            }
            catch (Exception ex)
            {
                return Ok(new { message = ex.Message, result = false });
            }
        }
    }
}