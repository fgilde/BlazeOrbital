export function init(elem) {
    enableColumnResizing(elem);

    const bodyClickHandler = event => {
        const path = event.path || (event.composedPath && event.composedPath());
        const columnOptionsElement = elem ? elem.querySelector('thead .column-options') : null;
        if (elem && columnOptionsElement && path.indexOf(columnOptionsElement) < 0) {
            elem.dispatchEvent(new CustomEvent('closecolumnoptions', { bubbles: true }));
        }
    };
    const keyDownHandler = event => {
        const columnOptionsElement = elem ? elem.querySelector('thead .column-options') : null;
        if (elem && columnOptionsElement && event.key === "Escape") {
            elem.dispatchEvent(new CustomEvent('closecolumnoptions', { bubbles: true }));
        }
    };

    document.body.addEventListener('click', bodyClickHandler);
    document.body.addEventListener('mousedown', bodyClickHandler); // Otherwise it seems strange that it doesn't go away until you release the mouse button
    document.body.addEventListener('keydown', keyDownHandler);

    return {
        stop: () => {
            document.body.removeEventListener('click', bodyClickHandler);
            document.body.removeEventListener('mousedown', bodyClickHandler);
            document.body.removeEventListener('keydown', keyDownHandler);
        }
    };
}

export function checkColumnOptionsPosition(elem) {
    const colOptions = elem ? elem.querySelector('.column-options') : null;
    if (elem && colOptions && colOptions.offsetLeft < 0) {
        colOptions.style.transform = `translateX(${ -1 * colOptions.offsetLeft }px)`;
    }
}

function enableColumnResizing(elem) {
    const elements = elem ? elem.querySelectorAll('thead .column-width-draghandle') || [] : [];
    elements.forEach(handle => {
        const th = handle.parentNode;
        handle.addEventListener('mousedown', evt => {
            evt.preventDefault();
            evt.stopPropagation();
            const startPageX = evt.pageX;
            const originalColumnWidth = th.offsetWidth;
            let updatedColumnWidth = 0;

            function handleMouseMove(evt) {
                evt.preventDefault();
                evt.stopPropagation();
                const nextWidth = originalColumnWidth + evt.pageX - startPageX;
                if (Math.abs(nextWidth - updatedColumnWidth) > 0) {
                    updatedColumnWidth = nextWidth;
                    th.style.width = `${updatedColumnWidth}px`;
                }
            }

            function handleMouseUp() {
                document.body.removeEventListener('mousemove', handleMouseMove);
                document.body.removeEventListener('mouseup', handleMouseUp);
            }

            document.body.addEventListener('mousemove', handleMouseMove);
            document.body.addEventListener('mouseup', handleMouseUp);
        });
    });
}