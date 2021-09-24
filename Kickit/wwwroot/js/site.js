//Sets the dropdown menu text to whatever option was selected
$(".dropdown-menu button").click(function () {
	var menuText = $(this).parents(".dropdown").find('.btn');
	var btnValue = $(this).data('value');

	menuText.html($(this).text() + '<span class="caret"></span>');
	menuText.val(btnValue);
	
	if (btnValue.contains("Completed")) {
		const div = document.querySelector("dropdown-toggle");

		if (div.classList.contains("btn-secondary")) {
			$(menuText).removeClass("btn-secondary").addClass("btn-success");
		}
		if (div.classList.contains("btn-warning")) {
			$(menuText).removeClass("btn-warning").addClass("btn-success");
		}
	}

	if (btnValue.contains("Not Completed")) {
		const div = document.querySelector("dropdown-toggle");

		if (div.classList.contains("btn-secondary")) {
			$(menuText).removeClass("btn-secondary").addClass("btn-warning");
		}
		if (div.classList.contains("btn-success")) {
			$(menuText).removeClass("btn-success").addClass("btn-warning");
		}
	}
});

$('.pword, .pword-confirm').on('keyup', function () {
	if ($('.pword').val() == $('.pword-confirm').val()) {
		$('.validation-for-confirm-password').html('');
	} else
		$('.validation-for-confirm-password').html('Your passwords must match.');
});