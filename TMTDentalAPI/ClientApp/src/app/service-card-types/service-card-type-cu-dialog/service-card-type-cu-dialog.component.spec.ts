import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardTypeCuDialogComponent } from './service-card-type-cu-dialog.component';

describe('ServiceCardTypeCuDialogComponent', () => {
  let component: ServiceCardTypeCuDialogComponent;
  let fixture: ComponentFixture<ServiceCardTypeCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardTypeCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardTypeCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
