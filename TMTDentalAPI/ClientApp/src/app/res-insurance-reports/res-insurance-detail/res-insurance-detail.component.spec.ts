import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceDetailComponent } from './res-insurance-detail.component';

describe('ResInsuranceDetailComponent', () => {
  let component: ResInsuranceDetailComponent;
  let fixture: ComponentFixture<ResInsuranceDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
