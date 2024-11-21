import {
    ClassicEditor,
    Essentials,
    Paragraph,
    Bold,
    Italic,
    Underline,
    Strikethrough,
    Font
} from 'ckeditor5';
let content;
ClassicEditor
    .create( document.querySelector( '#editor' ), {
        plugins: [Essentials, Paragraph, Bold, Italic, Underline, Strikethrough, Font],
        toolbar: [
            'undo', 'redo', '|', 'bold', 'italic', 'underline', 'strikethrough', '|',
            'fontSize', 'fontFamily', 'fontColor', 'fontBackgroundColor'
        ]
    } )
    .then( editor => {
        window.content = editor;
        console.log("content")
    } )
    .catch( error => {
        console.error( error );
    } );