import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboFinishLineListComponent } from './labo-finish-line-list.component';

describe('LaboFinishLineListComponent', () => {
  let component: LaboFinishLineListComponent;
  let fixture: ComponentFixture<LaboFinishLineListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboFinishLineListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboFinishLineListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
