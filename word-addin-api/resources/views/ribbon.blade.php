<customUI xmlns="http://schemas.microsoft.com/office/2009/07/customui">
  <ribbon>
    <tabs>
      <tab id="tabServer" label="{{ $tabLabel }}">
        <group id="grpDocuments" label="{{ $groupLabel }}">
@foreach ($buttons as $button)
          <button id="btn_{{ $button['id'] }}"
                  label="{{ $button['label'] }}"
                  size="large"
                  imageMso="FileNew"
                  tag="{{ $button['id'] }}"
                  onAction="OnAction" />
@endforeach
        </group>
      </tab>
    </tabs>
  </ribbon>
</customUI>