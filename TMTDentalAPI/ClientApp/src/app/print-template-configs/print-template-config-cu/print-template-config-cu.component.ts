import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GenerateReq, PrintTemplateConfigChangeType, PrintTemplateConfigDisplay, PrintTemplateConfigSave, PrintTemplateConfigService, PrintTestReq } from '../print-template-config.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { PrintTemplateDefault, PrintTemplateService } from '../print-template.service';

@Component({
    selector: 'app-print-template-config-cu',
    templateUrl: './print-template-config-cu.component.html',
    styleUrls: ['./print-template-config-cu.component.css']
})
export class PrintTemplateConfigCuComponent implements OnInit {
    typeFilter: any;
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

    constructor(private configService: PrintTemplateConfigService,
        private activeRoute: ActivatedRoute,
        private notifyService: NotifyService,
        private authService: AuthService,
        private printService: PrintService,
        private printTemplateService: PrintTemplateService
    ) { }

    ngOnInit() {
        this.types = this.configService.types;
        // this.typeFilter = "tmp_toathuoc";
        this.typeFilter = "tmp_purchase_order";
        this.loadCurrentConfig();
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

    loadCurrentConfig(preType?) {
        var val = new PrintTemplateConfigChangeType();
        val.type = this.typeFilter;
        this.configService.getDisplay(val).subscribe(res => {
            this.config = res;
            this.onGenerate(this.config.content);
            if (this.isEditting) {
                this.configEdit = Object.assign({}, this.config);
            }
        },
            err => {
                if (preType) this.typeFilter = preType;
                console.log(this.typeFilter);

            }
        );

    }


    onDefault() {
        var val = Object.assign({}, this.configEdit) as PrintTemplateDefault;
        val.type = this.typeFilter;
        this.printTemplateService.getDisplay(val).subscribe(res => {
            this.configEdit.content = res;
        });
    }

    onPrint() {
        if (this.isEditting) {
            this.printService.printHtml(this.contentPrev);
        } else {
            var val = new PrintTestReq();
            val.type = this.typeFilter;
            this.configService.printTest(val).subscribe(res => {
                this.printService.printHtml(res);
            });
        }
    }

    onSave() {
        var val = Object.assign({}, this.configEdit) as PrintTemplateConfigSave;
        val.companyId = this.authService.userInfo ? this.authService.userInfo.companyId : '';
        val.type = this.typeFilter;
        this.configService.createOrUpdate(val).subscribe(res => {
            this.notifyService.notify('success', 'Lưu thành công');
            this.onToggleEdit();
            this.config = this.configEdit;
        });
    }

    onChangeType(e) {
        var prev = this.typeFilter;
        this.typeFilter = e;
        this.loadCurrentConfig(prev);
    }

    onEdit() {
        this.configEdit = Object.assign({}, this.config);
        this.onToggleEdit();
    }

    onToggleEdit() {
        this.isEditting = !this.isEditting;
    }

    onGenerate(content?) {
        var val = new GenerateReq();
        val.content = content || this.configEdit.content;
        val.type = this.typeFilter;
        this.configService.generate(val).subscribe((res: any) => {
            this.contentPrev = res;
        });
    }
}
