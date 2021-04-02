import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { result } from 'lodash';
import { AdvisoryDefaultGet, AdvisoryService } from 'src/app/advisories/advisory.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import {ToothDiagnosisPopoverComponent} from '../partner-customer-advisory-list/tooth-diagnosis-popover/tooth-diagnosis-popover.component'

@Component({
  selector: 'app-partner-customer-advisory-cu-dialog',
  templateUrl: './partner-customer-advisory-cu-dialog.component.html',
  styleUrls: ['./partner-customer-advisory-cu-dialog.component.css']
})
export class PartnerCustomerAdvisoryCuDialogComponent implements OnInit {

  myForm: FormGroup;
  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];
  filteredToothCategories: any[] = [];
  cateId: string;
  submitted = false;
  customerId: string;
  id: string;

  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
    private advisoryService: AdvisoryService,
    private intlService: IntlService,
    private notificationService: NotificationService,
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      dateObj: [null, Validators.required],
      customerId: [null, Validators.required],
      customer: [null],
      userId: [null, Validators.required],
      user:[null],
      toothCategoryId:[null],
      teeth: [[], Validators.required],
      toothDiagnosis: [[], Validators.required],
      product: [[], Validators.required],
      note: [],
      companyId: [null,Validators.required]
    })
    setTimeout(() => {
      this.loadToothCategories();
      this.loadDefaultToothCategory().subscribe(result => {
        this.cateId = result.id;
        this.loadTeethMap(result);
      })
      if(this.id){
        this.getById();
      }else{
        this.getDefault();
      }
     
    }, 200);
    
  }

  get f(){
    return this.myForm.controls;
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(result => this.processTeeth(result));
  }

  processTeeth(teeth: ToothDisplay[]) {
    this.hamList = {
      '0_up': { '0_right': [], '1_left': [] },
      '1_down': { '0_right': [], '1_left': [] }
    };

    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.position === '1_left') {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
  }

  isSelected(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return true;
      }
    }

    return false;
  }

  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
    }
  }

  getSelectedIndex(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return i;
      }
    }

    return null;
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }

  loadToothCategories() {
    return this.toothCategoryService.getAll().subscribe(result => this.filteredToothCategories = result);
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
      this.cateId = value.id;
    }
  }

  onSave(){
    this.submitted = true;
    if (!this.myForm.valid) {
      return false;
    }

    console.log(this.myForm);
    var valueForm = this.myForm.value;
    valueForm.date = this.intlService.formatDate(valueForm.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    valueForm.toothCategoryId = this.cateId ;
    valueForm.toothIds = this.teethSelected.map(x => x.id);
    valueForm.toothDiagnosisIds = valueForm.toothDiagnosis.map(x => x.id);
    valueForm.productIds = valueForm.product.map(x => x.id);
    this.advisoryService.create(valueForm).subscribe(() => {
      this.notify("success","Lưu thành công");
      this.activeModal.close(true);
    })
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  getDefault(){
    var val = new AdvisoryDefaultGet();
    val.customerId = this.customerId
    this.advisoryService.getDefault(val).subscribe(result => {
      this.myForm.patchValue(result);
      let date = new Date(result.date);
      this.myForm.get('dateObj').patchValue(date);
    })
  }

  getById(){
    this.advisoryService.get(this.id).subscribe(result => {
      this.myForm.patchValue(result);
      let date = new Date(result.date);
      this.myForm.get('dateObj').patchValue(date);
   
      console.log(result);
      
      
    });
  }

  updateDiagnosis(data){ 
    this.f.toothDiagnosis.setValue(data);
  }

  updateProduct(data){
    this.f.product.setValue(data);
  }

  resetForm(){
    this.f.toothDiagnosis.setValue([]);
    this.f.product.setValue([]);
    this.f.teeth.setValue([]);
    this.teethSelected = [];
    this.f.note.reset();
    
  }
}
