/**
 * @license Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function (config) {
	// Define changes to default configuration here. For example:
	config.language = 'vi';
	config.defaultLanguage = 'vi',
	config.entities = false;
	config.allowedContent = true;

	config.toolbarGroups = [
		{ name: 'document', groups: [ 'mode', 'document', 'doctools' ] },
		{ name: 'styles', groups: [ 'styles' ] },
		{ name: 'colors', groups: [ 'colors' ] },
		'/',
		{ name: 'basicstyles', groups: [ 'basicstyles', 'cleanup' ] },
		{ name: 'paragraph', groups: [ 'list', 'indent', 'blocks', 'align', 'bidi', 'paragraph' ] },
		{ name: 'links', groups: [ 'links' ] },
		{ name: 'insert', groups: [ 'insert' ] },
		{ name: 'tools', groups: [ 'tools' ] },
	];

	config.removeButtons = 'Save,ExportPdf,Preview,Templates,Find,Replace,Scayt,Flash,Smiley,PageBreak,Iframe,CopyFormatting,RemoveFormat,Blockquote,JustifyBlock,Language,Form,Radio,Checkbox,TextField,Textarea,Select,Button,ImageButton,HiddenField,Link,Unlink,Anchor,About,Maximize,NewPage,Print,Cut,Copy,Paste,PasteText,PasteFromWord,Undo,Redo,SelectAll,Strike,BidiLtr,BidiRtl';
	config.removePlugins = 'divarea';
	config.contentsCss = ['/css/print.css', CKEDITOR.basePath + 'content.config.css'];
	config.bodyClass = 'container';
};
