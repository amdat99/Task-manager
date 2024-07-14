// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

const setDeleteListener = () => {
  $(".delete-button")
    .off("click")
    .on("click", function (e) {
      const id = e.target.id.split("-")[1];
      DisplayModal({
        content: "Are you sure you want to delete this task?",
        buttonText: "Delete",
        buttonStyle: "background-color: #dc3545",
        submitFunction: async () => {
          const response = await deleteTaskReq(id);
          if (!response?.success) return DisplayToast({ message: response?.message || "Error", type: "danger" });
          $(`#task-${id}`).remove();
          CloseModal();
          DisplayToast({ message: "Task deleted", type: "success" });
        },
      });
    });
};

const setCompleteListener = () => {
  $(".completed-task-checkbox")
    .off("change")
    .on("change", async function (e) {
      const id = e.target.id;
      //check if the task is completed or not
      const completed = $(this).is(":checked");
      const response = await toggleTaskCompletionReq(id.split("-")[1], completed);
      if (!response?.success) return DisplayToast({ message: response?.message || "Error", type: "danger" });
      DisplayToast({ message: "Task updated", type: "success" });
      // if completed, check checkbox , else uncheck
      $(this).prop("checked", completed);
    });
};

const deleteTaskReq = async (id) => {
  const response = await RequestHandler({
    method: "DELETE",
    url: `task/delete/${id}`,
  });
  return response;
};

const toggleTaskCompletionReq = async (id, completed) => {
  const response = await RequestHandler({
    method: "PATCH",
    url: `task/SetTask${completed ? "Completed" : "NotCompleted"}/${id}`,
  });
  return response;
};

$(document).ready(function () {
  setDeleteListener();
  setCompleteListener();
});
