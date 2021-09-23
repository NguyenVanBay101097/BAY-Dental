import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GenerateReq, PrintTemplateConfigChangePaperSize, PrintTemplateConfigDisplay, PrintTemplateConfigSave, PrintTemplateConfigService, PrintTestReq } from '../print-template-config.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { PrintTemplateDefault, PrintTemplateService } from '../print-template.service';
import { PrintPaperSizeBasic, PrintPaperSizeDisplay, PrintPaperSizePaged, PrintPaperSizeService } from 'src/app/config-prints/print-paper-size.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PrintPaperSizeCreateUpdateDialogComponent } from 'src/app/config-prints/print-paper-size-create-update-dialog/print-paper-size-create-update-dialog.component';
import * as _ from 'lodash';
import * as constantData from '../print-template-config-constant-data';
import { KeywordListDialogComponent } from '../keyword-list-dialog/keyword-list-dialog.component';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
// import { CKEditor4 } from 'ckeditor4-angular';
// import * as edit4 from '../../ckCustomBuild/ckeditor.js'
// var CKEDITOR_BASEPATH = '/ckeditor/';
@Component({
    selector: 'app-print-template-config-cu',
    templateUrl: './print-template-config-cu.component.html',
    styleUrls: ['./print-template-config-cu.component.css']
})
export class PrintTemplateConfigCuComponent implements OnInit {
    // public editor = Editor;
    types: { text: string, value: string }[] = [];
    // config = new PrintTemplateConfigDisplay();
    configEdit = new PrintTemplateConfigDisplay();
    configEditor = {
        // extraPlugins: "easyimage,dialogui,dialog,a11yhelp,about,basicstyles,bidi,blockquote,clipboard," +
        //     "button,panelbutton,panel,floatpanel,colorbutton,colordialog,menu," +
        //     "contextmenu,dialogadvtab,div,elementspath,enterkey,entities,popup," +
        //     "filebrowser,find,fakeobjects,flash,floatingspace,listblock,richcombo," +
        //     "font,format,forms,horizontalrule,htmlwriter,iframe,image,indent," +
        //     "indentblock,indentlist,justify,link,list,liststyle,magicline," +
        //     "maximize,newpage,pagebreak,pastefromword,pastetext,preview,print," +
        //     "removeformat,resize,save,menubutton,scayt,selectall,showblocks," +
        //     "showborders,smiley,sourcearea,specialchar,stylescombo,tab,table," +
        //     "tabletools,templates,toolbar,undo,wsc,wysiwygarea",
        height: 650,
        // fullPage: true,//this support html full page
        // allowedContent: true,
        // basicEntities: false,
        // forceSimpleAmpersand: true,
        // enterMode: 2,//this support not format string to p tag
        // protectedSource: [/{{[\s\S]*?}}/g] // this support loop code
        // skin: 'kama'
        // plugins: ['EasyImage'],

    };
    contentPrev = "";
    paperSizes: PrintPaperSizeBasic[] = [];
    filter = new PrintTemplateConfigChangePaperSize();
    formGroup: FormGroup;

    @ViewChild("editor", { static: false }) editor;

    constructor(private configService: PrintTemplateConfigService,
        private activeRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private authService: AuthService,
        private printService: PrintService,
        private printTemplateService: PrintTemplateService,
        private paperSizeService: PrintPaperSizeService,
        private modalService: NgbModal,
        private fb: FormBuilder
    ) { }

    ngOnInit() {
        this.formGroup = this.fb.group({
            type: [null, Validators.required],
            printPaperSizeId: [null, Validators.required],
            content: ''
        });

        this.types = constantData.types;
        this.loadPaperSizeList();
    }

    public onReady(editor) {
        editor.ui.getEditableElement().parentElement.insertBefore(
            editor.ui.view.toolbar.element,
            editor.ui.getEditableElement()
        );

    }

    loadPaperSizeList() {
        var val = new PrintPaperSizePaged();
        this.paperSizeService.getPaged(val).subscribe(res => {
            this.paperSizes = res.items;
        });
    }

    onDefault() {
        var type = this.formGroup.get('type').value;
        if (type) {
            let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
            modalRef.componentInstance.title = 'Sử dụng mẫu mặc định';
            modalRef.componentInstance.body = 'Nội dung sẽ chuyển về mẫu mặc định, bạn có chắc chắn?';
            modalRef.result.then(() => {
                this.printTemplateService.getPrintTemplateDefault({ type }).subscribe((res: any) => {
                    this.formGroup.get('content').setValue(res.content);
                    this.onGenerate();
                });
            });
        }
    }

    onPrint() {
        var val = new PrintTestReq();
        val.type = this.formGroup.get('type').value;
        val.content = this.content;
        val.printPaperSizeId = this.formGroup.get('printPaperSizeId').value;
        this.configService.printTest(val).subscribe(res => {
            this.printService.printHtml(res);
        });
    }

    get content() {
        return this.formGroup.get('content').value;
    }

    onSave() {
        if (this.formGroup.invalid) {
            return false;
        }

        var val = this.formGroup.value;
        val.companyId = this.authService.userInfo ? this.authService.userInfo.companyId : '';
        this.configService.createOrUpdate(val).subscribe(res => {
            this.notifyService.notify('success', 'Lưu thành công');
        });
    }

    onChangeType(e) {
        var type = this.formGroup.get('type').value;
        if (type) {
            this.configService.getDisplay(type).subscribe((res: any) => {
                this.formGroup.get('printPaperSizeId').setValue(res.printPaperSizeId);
                this.formGroup.get('content').setValue(res.printTemplateContent);
                this.onGenerate();
            });
        }
    }

    onEdit() {
        this.configEdit = Object.assign({}, this.configEdit);
        this.filter.printPaperSizeId = this.configEdit.printPaperSizeId;
    }

    onGenerate(content?) {
        if (this.formGroup.invalid) {
            return false;
        }

        var val = this.formGroup.value;
        this.configService.generate(val).subscribe((res: any) => {
            this.contentPrev = res;
        }, () => {
            this.contentPrev = 'Không thể hiển thị do sai cú pháp';
        });
    }

    onChangePaperSize(e) {
        var val = {
            type: this.formGroup.get('type').value,
            printPaperSizeId: this.formGroup.get('printPaperSizeId').value,
        };
        this.configService.changePaperSize(val).subscribe((res: any) => {
            this.formGroup.get('printPaperSizeId').setValue(res.printPaperSizeId);
            this.formGroup.get('content').setValue(res.printTemplateContent);
            this.onGenerate();
        });
    }

    onCreatePaperSize() {
        const modalRef = this.modalService.open(PrintPaperSizeCreateUpdateDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Thêm khổ giấy in';
        modalRef.result.then((res) => {
            this.paperSizes = _.unionBy(this.paperSizes, [res], 'id');
            this.filter.printPaperSizeId = res.id;
            this.onGenerate();
        }, () => {
        });
    }

    onEditPaperSize() {
        const modalRef = this.modalService.open(PrintPaperSizeCreateUpdateDialogComponent, { scrollable: true, windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Sửa khổ giấy in';
        modalRef.componentInstance.id = this.filter.printPaperSizeId;
        modalRef.result.then((res) => {
            this.paperSizes = _.unionBy(this.paperSizes, [res], 'id');
            this.onGenerate();
        }, () => {
        });
    }

    onAddKeyWord() {
        const modalRef = this.modalService.open(KeywordListDialogComponent, { size: 'xl', scrollable: true, windowClass: 'o_technical_modal', keyboard: true, backdrop: 'static' });
        modalRef.componentInstance.boxKeyWordSource = constantData.keyWords[this.formGroup.get('type').value];
        modalRef.result.then((res) => {
            if (res) {
                this.editor.instance.insertText(res.value);
                setTimeout(() => {
                    this.editor.instance.focus();
                }, 0);
            }
        }, () => {
        });

    }
}
