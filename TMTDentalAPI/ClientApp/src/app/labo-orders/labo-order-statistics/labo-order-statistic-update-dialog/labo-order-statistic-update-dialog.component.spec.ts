import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderStatisticUpdateDialogComponent } from './labo-order-statistic-update-dialog.component';

describe('LaboOrderStatisticUpdateDialogComponent', () => {
  let component: LaboOrderStatisticUpdateDialogComponent;
  let fixture: ComponentFixture<LaboOrderStatisticUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderStatisticUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderStatisticUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
