import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToaThuocLineSaveComponent } from './toa-thuoc-line-save.component';

describe('ToaThuocLineSaveComponent', () => {
  let component: ToaThuocLineSaveComponent;
  let fixture: ComponentFixture<ToaThuocLineSaveComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToaThuocLineSaveComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToaThuocLineSaveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
