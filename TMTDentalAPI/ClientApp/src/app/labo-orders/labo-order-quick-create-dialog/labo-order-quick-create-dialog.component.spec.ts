import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderQuickCreateDialogComponent } from './labo-order-quick-create-dialog.component';

describe('LaboOrderQuickCreateDialogComponent', () => {
  let component: LaboOrderQuickCreateDialogComponent;
  let fixture: ComponentFixture<LaboOrderQuickCreateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderQuickCreateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderQuickCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
