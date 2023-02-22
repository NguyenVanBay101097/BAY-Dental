import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardOrderPaymentsDialogComponent } from './service-card-order-payments-dialog.component';

describe('ServiceCardOrderPaymentsDialogComponent', () => {
  let component: ServiceCardOrderPaymentsDialogComponent;
  let fixture: ComponentFixture<ServiceCardOrderPaymentsDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardOrderPaymentsDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardOrderPaymentsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
