import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSupplierFormComponent } from './partner-supplier-form.component';

describe('PartnerSupplierFormComponent', () => {
  let component: PartnerSupplierFormComponent;
  let fixture: ComponentFixture<PartnerSupplierFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSupplierFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSupplierFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
