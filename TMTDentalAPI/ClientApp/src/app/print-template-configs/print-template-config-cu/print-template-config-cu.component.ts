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

@Component({
    selector: 'app-print-template-config-cu',
    templateUrl: './print-template-config-cu.component.html',
    styleUrls: ['./print-template-config-cu.component.css']
})
export class PrintTemplateConfigCuComponent implements OnInit {
    types: { text: string, value: string }[] = [];
    // config = new PrintTemplateConfigDisplay();
    configEdit = new PrintTemplateConfigDisplay();
    configEditor = {
        language: 'vi',
        height: 650,
        contentsCss: '/css/print.css',
        // fullPage: true,//this support html full page
        allowedContent: true,
        entities: false,
        basicEntities: false,
        forceSimpleAmpersand: true,
        enterMode: 2,//this support not format string to p tag
        // protectedSource: [/{{[\s\S]*?}}/g] // this support loop code
        // skin: 'kama'
    };
    contentPrev = "";
    paperSizes: PrintPaperSizeBasic[] = [];
    filter = new PrintTemplateConfigChangePaperSize();

    @ViewChild("editor", { static: false }) editor;

    constructor(private configService: PrintTemplateConfigService,
        private activeRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private authService: AuthService,
        private printService: PrintService,
        private printTemplateService: PrintTemplateService,
        private paperSizeService: PrintPaperSizeService,
        private modalService: NgbModal,

    ) { }

    ngOnInit() {
        this.types = constantData.types;
        this.filter.type = "tmp_toathuoc";
        this.loadCurrentConfig();
        this.loadPaperSizeList();
        // this.activeRoute.paramMap.subscribe(x => {
        //   this.typeFilter = x.get("type");
        //   this.loadCurrentConfig();
        // });
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

    loadCurrentConfig(preType?) {
        var val = Object.assign({}, this.filter);
        this.configService.getDisplay(val).subscribe(res => {
            this.configEdit = res;
            this.filter.printPaperSizeId = this.configEdit.printPaperSizeId;
            this.onGenerate(this.configEdit.content);
        },
            err => {
                if (preType) this.filter.type = preType;
            }
        );

    }


    onDefault() {
        var val = Object.assign({}, this.configEdit) as PrintTemplateDefault;
        val.type = this.filter.type;
        this.printTemplateService.getDisplay(val).subscribe(res => {
            this.configEdit.content = res;
            this.onGenerate();
        });
    }

    onPrint() {
        var val = new PrintTestReq();
        val.type = this.filter.type;
        val.content = this.configEdit.content;
        val.printPaperSizeId = this.filter.printPaperSizeId || '';
        this.configService.printTest(val).subscribe(res => {
            this.printService.printHtml(res);
        });
    }

    onSave() {
        var val = Object.assign({}, this.configEdit) as PrintTemplateConfigSave;
        val.companyId = this.authService.userInfo ? this.authService.userInfo.companyId : '';
        val.type = this.filter.type;
        this.configService.createOrUpdate(val).subscribe(res => {
            this.notifyService.notify('success', 'Lưu thành công');
        });
    }

    onChangeType(e) {
        var val = Object.assign({}, this.filter);
        val.type = e;
        this.configService.changePaperSize(val).subscribe((res: any) => {
            this.configEdit = res;
            this.filter.type = e;
            this.onGenerate(this.configEdit.content);
        });
    }

    onEdit() {
        this.configEdit = Object.assign({}, this.configEdit);
        this.filter.printPaperSizeId = this.configEdit.printPaperSizeId;
    }

    onGenerate(content?) {
        var val = Object.assign({}, this.filter) as GenerateReq;
        val.content = content || this.configEdit.content;
        this.configService.generate(val).subscribe((res: any) => {
            this.contentPrev = res;
        }, () => {
            this.contentPrev = 'Không thể hiển thị do sai cú pháp';
        });
    }

    onChangePaperSize(e) {
        this.filter.printPaperSizeId = e;
        var val = Object.assign({}, this.filter);
        this.configService.changePaperSize(val).subscribe((res: any) => {
            this.configEdit = res;
            this.onGenerate(this.configEdit.content);
        });
    }

    onCancel() {
        delete this.filter.printPaperSizeId;
        this.loadCurrentConfig();
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
        modalRef.componentInstance.boxKeyWordSource = constantData.keyWords[this.filter.type];
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
