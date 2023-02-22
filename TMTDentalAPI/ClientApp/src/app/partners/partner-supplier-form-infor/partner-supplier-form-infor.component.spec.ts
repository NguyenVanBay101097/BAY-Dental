import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSupplierFormInforComponent } from './partner-supplier-form-infor.component';

describe('PartnerSupplierFormInforComponent', () => {
  let component: PartnerSupplierFormInforComponent;
  let fixture: ComponentFixture<PartnerSupplierFormInforComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSupplierFormInforComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSupplierFormInforComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
