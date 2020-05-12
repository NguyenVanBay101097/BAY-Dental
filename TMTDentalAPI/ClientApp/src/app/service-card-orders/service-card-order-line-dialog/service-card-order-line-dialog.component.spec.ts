import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardOrderLineDialogComponent } from './service-card-order-line-dialog.component';

describe('ServiceCardOrderLineDialogComponent', () => {
  let component: ServiceCardOrderLineDialogComponent;
  let fixture: ComponentFixture<ServiceCardOrderLineDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardOrderLineDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardOrderLineDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
