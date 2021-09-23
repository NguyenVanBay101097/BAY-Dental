/**
 * @license Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function (config) {
	// Define changes to default configuration here. For example:
	config.language = 'vi';
	config.defaultLanguage = 'vi',
		config.enterMode = CKEDITOR.ENTER_BR;
	// config.uiColor = '#AADC6E';
	config.entities = false;
	config.toolbar = [
		{ name: 'document', items: ['Source'] },
		{ name: 'styles', items: ['Format', 'Font', 'FontSize'] },
		{ name: 'colors', items: ['TextColor', 'BGColor'] },
		{ name: 'insert', items: ['Image', 'Table', 'HorizontalRule', 'Smiley'] },
		{ name: 'basicstyles', items: ['Bold', 'Italic', 'Underline'] },
		{ name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] }
	]

};
