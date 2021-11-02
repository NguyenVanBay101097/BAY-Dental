import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardTypeApplyDialogComponent } from './service-card-type-apply-dialog.component';

describe('ServiceCardTypeApplyDialogComponent', () => {
  let component: ServiceCardTypeApplyDialogComponent;
  let fixture: ComponentFixture<ServiceCardTypeApplyDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardTypeApplyDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardTypeApplyDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
