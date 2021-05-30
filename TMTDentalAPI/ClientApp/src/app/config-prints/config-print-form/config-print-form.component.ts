import { Component, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ConfigPrintService } from '../config-print.service';
import { PrintPaperSizeBasic, PrintPaperSizePaged, PrintPaperSizeService } from '../print-paper-size.service';

@Component({
  selector: 'app-config-print-form',
  templateUrl: './config-print-form.component.html',
  styleUrls: ['./config-print-form.component.css']
})
export class ConfigPrintFormComponent implements OnInit {
  formGroup: FormGroup;
  filterdPaperSizes: PrintPaperSizeBasic[] = [];
  
  @ViewChild('papersizeCbx', { static: true }) papersizeCbx: ComboBoxComponent;
  constructor(
    private fb: FormBuilder,
    private configPrintService: ConfigPrintService,
    private printPaperSizeService: PrintPaperSizeService,
    private authService: AuthService,
    public notifyService: NotifyService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      configs: this.fb.array([]),
    });

    setTimeout(() => {
      this.loadPaperSizes();
    });

    this.loadDataFromApi();
   
  }


  loadDataFromApi() {
    this.configPrintService.get().subscribe((res: any) => {
      let control = this.formGroup.get('configs') as FormArray;
      control.clear();
      res.forEach(config => {
        var g = this.fb.group(config);
        control.push(g);
      });
    });
  }

  loadPaperSizes() {
    this.searchPaperSizes().subscribe((result) => {
      this.filterdPaperSizes = _.unionBy(this.filterdPaperSizes, result.items, "id");
    });
  }

  searchPaperSizes(q?: string) {
    var val = new PrintPaperSizePaged();
    val.search = q || '';  
    return this.printPaperSizeService.getPaged(val);
  }

  get configs() {
    return this.formGroup.get('configs') as FormArray;
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.get('configs').value;
    val = this.repareConfigs(val);
    this.configPrintService.createOrUpdate(val).subscribe(() => {
      this.notifyService.notify('success', 'Lưu thành công');
      this.loadDataFromApi();
    }, err => {
    });

  }

  repareConfigs(val){
    val.forEach(config => {
      config.name = config.code;
      config.paperSizeId = config.printPaperSize ? config.printPaperSize.id : null;
      config.companyId = this.authService.userInfo.companyId;
    });
    return val;
  }


}
