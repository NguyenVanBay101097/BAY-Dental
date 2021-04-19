import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { result, values } from 'lodash';
import { AdvisoryDefaultGet, AdvisoryService } from 'src/app/advisories/advisory.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { ToothDiagnosisService } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';
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
  teethSelectedById: ToothDisplay[] = [];
  filteredToothCategories: any[] = [];
  productSelectedFromApi: any[] = [];
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
    private toothDiagnosisService: ToothDiagnosisService,
    private router: Router
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
    })

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
    this.f.teeth.setValue(this.teethSelected);
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
      this.f.teeth.setValue(this.teethSelected);
      this.loadTeethMap(value);
      this.cateId = value.id;
    }
  }

  onSave(){
    this.submitted = true;
    if (!this.myForm.valid) {
      return false;
    }
    var valueForm = this.myForm.value;
    valueForm.date = this.intlService.formatDate(valueForm.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    valueForm.toothCategoryId = this.cateId ;
    valueForm.toothIds = this.teethSelected.map(x => x.id);
    valueForm.toothDiagnosisIds = valueForm.toothDiagnosis.map(x => x.id);
    valueForm.productIds = valueForm.product.map(x => x.id);
    if(this.id){
      this.advisoryService.update(valueForm,this.id).subscribe(() => {
        this.notify("success","Lưu thành công");
        this.activeModal.close(true);
      })
    }else{
      this.advisoryService.create(valueForm).subscribe(() => {
        this.notify("success","Lưu thành công");
        this.activeModal.close(true);
      })
    }
    
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
      this.cateId = result.toothCategoryId;
      this.loadTeethMap(result.toothCategory);
      this.teethSelected = result.teeth;
      this.teethSelectedById = result.teeth;
      this.myForm.patchValue(result);
      let date = new Date(result.date);
      this.myForm.get('dateObj').patchValue(date);
      var ids = this.f.toothDiagnosis.value.map(x => x.id);
      this.toothDiagnosisService.getProducts(ids).subscribe(result => {
        this.productSelectedFromApi = result;
      })
    });
  }


  updateDiagnosis(data){
    this.f.toothDiagnosis.setValue(data);
    var ids = data.map(x => x.id);
    this.toothDiagnosisService.getProducts(ids).subscribe(result => {
      var products = this.f.product.value;
      products = products.filter(elem => this.productSelectedFromApi.findIndex(x => x.id == elem.id) == -1);
      products = products.concat(result);
      var unique = products.filter(function(elem, index, self){
        return index === self.findIndex(x => x.id == elem.id);
      })
      this.f.product.setValue(unique);
      this.productSelectedFromApi = result;
    })
  }
  updateProduct(data){
    this.f.product.setValue(data);
  }

  resetForm(){
    if(this.id){
      this.getById();
    }
    else{
      this.f.toothDiagnosis.setValue([]);
      this.f.product.setValue([]);
      this.teethSelected = [];
      this.f.teeth.setValue(this.teethSelected);
      this.f.note.reset();
    }
    
  }

  viewPartner(){
    this.router.navigate(['/partners/customer/'+this.customerId+'/overview']);
    this.activeModal.dismiss();
  }
}
