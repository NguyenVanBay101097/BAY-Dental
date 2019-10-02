import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DotKhamLineCuDialogComponent } from './dot-kham-line-cu-dialog.component';

describe('DotKhamLineCuDialogComponent', () => {
  let component: DotKhamLineCuDialogComponent;
  let fixture: ComponentFixture<DotKhamLineCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DotKhamLineCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DotKhamLineCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
