import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardTypeApplyAllComponent } from './service-card-type-apply-all.component';

describe('ServiceCardTypeApplyAllComponent', () => {
  let component: ServiceCardTypeApplyAllComponent;
  let fixture: ComponentFixture<ServiceCardTypeApplyAllComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardTypeApplyAllComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardTypeApplyAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
