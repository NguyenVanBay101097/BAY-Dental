import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceHistoriesDetailComponent } from './res-insurance-histories-detail.component';

describe('ResInsuranceHistoriesDetailComponent', () => {
  let component: ResInsuranceHistoriesDetailComponent;
  let fixture: ComponentFixture<ResInsuranceHistoriesDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceHistoriesDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceHistoriesDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
