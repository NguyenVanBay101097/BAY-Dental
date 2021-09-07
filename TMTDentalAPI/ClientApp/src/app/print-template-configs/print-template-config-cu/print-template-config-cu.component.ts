import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GenerateReq, PrintTemplateConfigChangeType, PrintTemplateConfigDisplay, PrintTemplateConfigSave, PrintTemplateConfigService, PrintTestReq } from '../print-template-config.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { PrintTemplateDefault, PrintTemplateService } from '../print-template.service';
import { PrintPaperSizeBasic, PrintPaperSizeDisplay, PrintPaperSizePaged, PrintPaperSizeService } from 'src/app/config-prints/print-paper-size.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PrintPaperSizeCreateUpdateDialogComponent } from 'src/app/config-prints/print-paper-size-create-update-dialog/print-paper-size-create-update-dialog.component';
import * as _ from 'lodash';

@Component({
    selector: 'app-print-template-config-cu',
    templateUrl: './print-template-config-cu.component.html',
    styleUrls: ['./print-template-config-cu.component.css']
})
export class PrintTemplateConfigCuComponent implements OnInit {
    isEditting = false;
    types: { text: string, value: string }[] = [];
    config = new PrintTemplateConfigDisplay();
    configEdit = new PrintTemplateConfigDisplay();
    configEditor = {
        language: 'vi',
        height: 525,
        contentsCss: '/css/print.css',
        fullPage: true,
        allowedContent: true,
        entities: false,
        basicEntities: false,
        forceSimpleAmpersand: true
    };
    contentPrev = "";
    paperSizes: PrintPaperSizeBasic[] = [];
    filter = new PrintTemplateConfigChangeType();

    constructor(private configService: PrintTemplateConfigService,
        private activeRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private authService: AuthService,
        private printService: PrintService,
        private printTemplateService: PrintTemplateService,
        private paperSizeService: PrintPaperSizeService,
        private modalService: NgbModal
    ) { }

    ngOnInit() {
        this.types = this.configService.types;
        this.filter.type = "tmp_medicine_order";
        this.filter.isDefault = true;
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
            this.config = res;
            this.filter.printPaperSizeId = this.config.printPaperSizeId;
            this.onGenerate(this.config.content);
            if (this.isEditting) {
                this.configEdit = Object.assign({}, this.config);
            }
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
        if (this.isEditting) {
            this.printService.printHtml(this.contentPrev);
        } else {
            var val = new PrintTestReq();
            val.type = this.filter.type;
            this.configService.printTest(val).subscribe(res => {
                this.printService.printHtml(res);
            });
        }
    }

    onSave() {
        var val = Object.assign({}, this.configEdit) as PrintTemplateConfigSave;
        val.companyId = this.authService.userInfo ? this.authService.userInfo.companyId : '';
        val.type = this.filter.type;
        this.configService.createOrUpdate(val).subscribe(res => {
            this.notifyService.notify('success', 'Lưu thành công');
            this.onToggleEdit();
            this.config = this.configEdit;
        });
    }

    onChangeType(e) {
        var prev = this.filter.type;
        this.filter.type = e;
        this.loadCurrentConfig(prev);
    }

    onEdit() {
        this.configEdit = Object.assign({}, this.config);
        this.filter.printPaperSizeId = this.config.printPaperSizeId;
        delete this.filter.isDefault;
        this.onToggleEdit();
    }

    onToggleEdit() {
        this.isEditting = !this.isEditting;
    }

    onGenerate(content?) {
        var val = Object.assign({}, this.filter) as GenerateReq;
        val.content = content || this.configEdit.content;
        this.configService.generate(val).subscribe((res: any) => {
            this.contentPrev = res;
        });
    }

    onChangePaperSize(e) {
        this.filter.printPaperSizeId = e;
        this.loadCurrentConfig();
    }

    onCancel() {
        this.onToggleEdit();
        this.filter.isDefault = true;
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
}
