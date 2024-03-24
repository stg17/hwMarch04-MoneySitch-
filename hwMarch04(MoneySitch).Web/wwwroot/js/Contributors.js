$(() => {
    $("#new-contributor").on('click', function () {
        SetUpNewModal();
        new bootstrap.Modal($(".new-contrib")[0]).show();
    });

    $("#table").on('click', '.edit-contrib', function () {
        let modal = new bootstrap.Modal($(".new-contrib")[0]);
        const contributor = {
            firstName: $(this).data('first-name'), 
            lastName: $(this).data('last-name'),   
            cellNumber: $(this).data('cell'),
            alwaysInclude: $(this).data('always-include'),
            id: $(this).data('id')
        }
        SetUpEditModal(contributor);
        modal.show()
    });

    const SetUpNewModal = function () {
        $(".modal-title").text("New Contributor");
        $('#initialDepositDiv').show();
        $("#contributor_first_name").val('');
        $("#contributor_last_name").val('');
        $("#contributor_cell_number").val('');
        $("#contributor_created_at").val('');
        $("#contributor_always_include").val('true');
        $("form").attr('action', "/home/newContributor");
        $("#contributor_always_include").prop('checked', false);
    }

    const SetUpEditModal = function (contributor) {
        $(".modal-title").text("Edit Contributor");
        $('#initialDepositDiv').hide();
        $("#contributor_first_name").val(contributor.firstName);
        $("#contributor_last_name").val(contributor.lastName);
        $("#contributor_cell_number").val(contributor.cellNumber);
        $("#contributor_created_at").val();
        if (contributor.alwaysInclude === 'True') {
            $("#contributor_always_include").prop('checked', true);
        }
        else {
            $("#contributor_always_include").prop('checked', false);
        }

        /*$("#contributor_always_include").prop('checked', !!contributor.alwaysInclude);*/
        $("form").attr('action', "/home/editcontrib");
        $("#form-submit").append(`<input type="hidden" name="Id" value="${contributor.id}" />`);
    }

    $("#table").on('click', '.deposit-button', function () {
        let name = $(this).data('name');
        let id = $(this).data('contribid');
        $("#deposit-name").text(`${name}`);
        $("#contributor-id").val(`${id}`);
        new bootstrap.Modal($("#deposit-modal")[0]).show();

    });

    $("#search").on('input', function () {
        ClearSearch();
        Search($("#search").val().toLowerCase())
    });

    $("#clear").on('click', function () {
        ClearSearch();
        $("#search").val("");
    });
    
    const Search = function (search) {
        $("tr").each(function () {
            const row = $(this);
            const button = row.find(".deposit-button");
            const name = button.data('name');
            
            if (name !== undefined && !name.toLowerCase().match(search)) {
                row.attr('style', "display: none;");
            }
        })
    };

    const ClearSearch = function () {
        $("tr").each(function () {
            const row = $(this);
            row.removeAttr('style');
        })
    }
    
})