import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceListComponent } from './res-insurance-list.component';

describe('ResInsuranceListComponent', () => {
  let component: ResInsuranceListComponent;
  let fixture: ComponentFixture<ResInsuranceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
