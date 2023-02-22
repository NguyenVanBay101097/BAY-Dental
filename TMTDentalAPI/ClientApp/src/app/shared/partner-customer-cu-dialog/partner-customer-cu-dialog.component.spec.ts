import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerCuDialogComponent } from './partner-customer-cu-dialog.component';

describe('PartnerCustomerCuDialogComponent', () => {
  let component: PartnerCustomerCuDialogComponent;
  let fixture: ComponentFixture<PartnerCustomerCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
