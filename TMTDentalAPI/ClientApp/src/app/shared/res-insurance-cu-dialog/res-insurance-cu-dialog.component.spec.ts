import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceCuDialogComponent } from './res-insurance-cu-dialog.component';

describe('ResInsuranceCuDialogComponent', () => {
  let component: ResInsuranceCuDialogComponent;
  let fixture: ComponentFixture<ResInsuranceCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
